# -*- coding: UTF-8 -*- 

import os
import sys
reload(sys)
sys.setdefaultencoding('utf-8')
import json
import xlrd
import google
import zlib
from Crypto.Cipher import DES
from Crypto.Cipher import AES
import binascii
import struct
import shutil
import gzip
import  codecs
from cStringIO import StringIO
from hashlib import md5


IS_DEBUG=False

def debug_log(*a) :
    if (IS_DEBUG) :
        print(a)

def load_json(file) :
    if not os.path.exists(file) :
        return {}
    f = open(file, "r")
    j = json.load(f, encoding='UTF-8')
    f.close()
    return j

def save_json(file, j) :
    sj = json.dumps(j, sort_keys=True,indent=2)
    f = open(file, "w+")
    f.write(sj)
    f.close()
    
PRIVATE_KEY="dd7fd4a156d28bade96f816db1d18609".decode("hex")
#PRIVATE_KEY="dd7fd4a156d28bad".decode("hex")
IV="dd7fd4a156d28bade96f816db1d18609".decode("hex")
#IV="dd7fd4a156d28bad".decode("hex")
ENCRYPT_OP = 1
DECRYPT_OP = 0

SERVER_DIR = "/frame/src/un/"

EXCEL_DIR = sys.argv[1]

if len(sys.argv) > 2 :
    LANG_PRE = sys.argv[2]
else :
    LANG_PRE = ""

SCRIPT_DIR=os.path.dirname(sys.argv[0])
debug_log(SCRIPT_DIR)


md5_config = load_json(EXCEL_DIR + "/md5_config.json")
md5_dconfig = load_json(EXCEL_DIR + "/md5_config.json")

configFile = open(SCRIPT_DIR + "/trans.json")
config = json.load(configFile, encoding='UTF-8')

if not os.path.exists(EXCEL_DIR + "/config.json"):
    shutil.copy(EXCEL_DIR + "/config_tmpl.json", EXCEL_DIR + "/config.json")
localConfigFile = open(EXCEL_DIR + "/config.json")
localConfig = json.load(localConfigFile, encoding='UTF-8')

keywordlines = open(EXCEL_DIR + "/keyword.txt", "r").readlines()
keywords = {}
for line in keywordlines :
    (k,v) = line.split()
    keywords[k.decode("UTF-8")] = v
    print(k.decode("UTF-8") + v)


exec("import " + config['proto']) 
exec("proto = " + config['proto'])

def genDesc(pre, firstRow, fields, isCheck) :
    desc = {}
    has = False 
    for field in fields :
        if field.label == field.LABEL_REPEATED :
            desc[field.name] = []
            i = 0 
            while True :
                path = pre + field.name + str(i)
                if field.type == field.TYPE_MESSAGE :
                    debug_log("gen repeated message", path)
                    subDesc = genDesc(path, firstRow, field.message_type.fields, False)
                    if subDesc :
                        desc[field.name].append(subDesc)
                    else :
                        break
                        
                else : 
                    path = pre + field.name + str(i)
                    rpath = path
                    if (LANG_PRE + rpath) in firstRow :
                        rpath = LANG_PRE + rpath
                    if path not in firstRow :
                        break 
                    desc[field.name].append({'path':rpath, 'col':firstRow.index(rpath), 'type':field.type})
                has = True
                i = i + 1
        else :
            path = pre + field.name
            if field.type == field.TYPE_MESSAGE : 
                debug_log("gen message", path)
                subDesc = genDesc(path, firstRow, field.message_type.fields, isCheck)
                if subDesc :
                    desc[field.name] = subDesc
                    has = True
                else :
                    assert(isCheck == False)
            else : 
                rpath = path
                debug_log("gen not message", path)
                if (LANG_PRE + rpath) in firstRow :
                    rpath = LANG_PRE + rpath
                if rpath in firstRow :
                    desc[field.name] ={'path':rpath, 'col':firstRow.index(rpath), 'type':field.type}
                    has = True
                else :
                    debug_log(field.label, field.LABEL_REQUIRED)
                    if isCheck and field.label == field.LABEL_REQUIRED :
                        print(rpath + " is required", firstRow)
                        assert(False)
    
    if has :
        return desc 
    else :
        return None

def setBaseValue(eData, name, row, desc) :
    setattr(eData, name, row[desc['col']])

debug_log(proto)

def my_float(value) :
    value = str(value)
    if value == '' :
        return 0
    else :
        return float(value)
        
def my_int(value) :
    return int(my_float(value))
    
def getValue(row, entry) :
    has = True
    
    value = row[entry['col']].value
    if value == '' :
        has = False
    
#    print(value)
#    print(keywords)
    if value in keywords :
        value = keywords[value]

    ctype = entry['type']
    #debug_log("ctype", ctype, value)
    if ctype == google.protobuf.descriptor.FieldDescriptor.TYPE_DOUBLE or ctype == google.protobuf.descriptor.FieldDescriptor.TYPE_FLOAT :
        return (my_float(value), has)
    elif ctype ==  google.protobuf.descriptor.FieldDescriptor.TYPE_STRING :
        if isinstance(value, (int, float)) : 
            value = str(int(value))
        return (value, has)
    else : 
        return (my_int(value), has)

def setValue(eData, row, desc, tconfig) :
    has = False
    for name, entry in desc.items() : 
        subData = getattr(eData, name)
        if isinstance(entry, list) : 
            for sentry in entry : 
                if 'col' in sentry : 
                    value, subHas = getValue(row, sentry)
                    if subHas or ("keepArray" in tconfig and tconfig["keepArray"]) : 
                        subData.append(value) 
                        has = True
                    else :
                        continue
                else : 
                    aData = subData.add()
                    subHas = setValue(aData, row, sentry, tconfig) 
                    if subHas or ("keepArray" in tconfig and tconfig["keepArray"]) : 
                    #if subHas :
                        has = True 
                    else :
                        subData.remove(aData)
        else : 
            if 'col' in entry :
                #debug_log("start", name)
                value, subHas = getValue(row, entry)
                #debug_log(name, value)
                setattr(eData, name, value)
                if subHas :
                    has = True     
            else : 
                subHas = setValue(subData, row, entry, tconfig) 
                if subHas :
                    has = True

    return has 

def encrypte(data) :
    aes = AES.new(PRIVATE_KEY, AES.MODE_CBC, IV)
    #aes = DES.new(PRIVATE_KEY, DES.MODE_CBC, IV)
    dlen = len(data)
    x = len(data) % 16
    if x != 0:
        data = data + '0'*(16 - x)
    edata = aes.encrypt(data)
    return struct.pack("!l", dlen) + edata
    #return data
    
def decrypte(edata) :
    (len,) = struct.unpack("!l", edata[:4])
    aes = AES.new(PRIVATE_KEY, AES.MODE_CBC, IV)
    ddata = aes.decrypt(edata[4:])
    return ddata[:len]
    #return edata

def dumpPB(tData, path) :
    pkg = tData.SerializeToString()
    zpkg = zlib.compress(pkg)
    #zbuf = StringIO()
    #zf = gzip.GzipFile(mode="wb", fileobj=zbuf)
    #zf.write(pkg)
    #zf.close()
    #zpkg = zbuf.getvalue()
    #zpkg=pkg
    epkg = encrypte(zpkg)
    #epkg = zpkg
    print("pkg:", path, len(epkg), len(pkg), len(zpkg))
    f = open(path, "wb+")
    f.write(epkg)
    f.close()
    #debug_log("pkg len:", len(pkg))
    #debug_log("zpkg len:", len(zpkg))
    #debug_log(binascii.b2a_hex(zpkg))
    #debug_log("epkg len:", len(epkg))
    #debug_log(binascii.b2a_hex(epkg))
    #zzpkg = decrypte(epkg)
    #debug_log("zzpkg len:", len(zzpkg))
    #debug_log(binascii.b2a_hex(zzpkg))
    #eepkg = encrypte(epkg)
    #debug_log("eepkg len:", len(eepkg))
    #debug_log(binascii.b2a_hex(eepkg))


def PBData2Struct(tData) :
    tStruct = {}
    fields = tData.ListFields()
    for type in tData.DESCRIPTOR.fields :
        if type.has_default_value :
            tStruct[type.name] = type.default_value
        if type.label == type.LABEL_REPEATED or type.type == type.TYPE_MESSAGE :
            tStruct[type.name] = []
    for (type, sData) in fields :
        if type.label == type.LABEL_REPEATED :
            tStruct[type.name] = []
            #debug_log("type", type.type)
            for entry in sData :
                #debug_log(entry)
                if type.type == type.TYPE_MESSAGE :
                    #debug_log("is message")
                    stStruct = PBData2Struct(entry)
                    tStruct[type.name].append(stStruct)
                else :
                    tStruct[type.name].append(entry)
        else :
            if type.type == type.TYPE_MESSAGE :
                #debug_log("is message")
                stStruct = PBData2Struct(sData)
                tStruct[type.name] = stStruct
            else :
                tStruct[type.name] = sData
    return tStruct
    
def dumpJson(tData, path) :
    tStruct = PBData2Struct(tData)
    encodedjson = json.dumps(tStruct, sort_keys=True,indent=2,ensure_ascii=False)
    f = codecs.open(path, "wb+", "utf-8")
    f.write(encodedjson)
    f.close()
    #debug_log encodedjson

def writeLua(tStruct, f, pre, tail) :
    if isinstance(tStruct, (int, float)) :
        f.write(str(tStruct))
    elif isinstance(tStruct, basestring) :
        f.write('"')
        f.write(tStruct.encode("utf-8"))
        f.write('"')
    elif isinstance(tStruct, list) :
        f.write("{\n")
        for v in tStruct :
            f.write(pre + "\t")
            writeLua(v, f, pre + "\t", ",\n")
        f.write(pre + "}")
    else : 
        f.write("{\n")
        for k in tStruct :
            v = tStruct[k]
            if isinstance(k, (int, float)) :
                f.write(pre + "\t[" + str(k) + "] = ")
            else :
                f.write(pre + "\t" + k + " = ")
                writeLua(v, f, pre + "\t", ",\n")
        f.write(pre + "}")
    
    f.write(tail)

def dumpLua(tData, path) :
    tStruct = PBData2Struct(tData)
    f = open(path, "w+")
    f.write("return ")
    writeLua(tStruct, f, "", "\n")
    f.close()
    
    f = open(path, "r") 
    try:
        source_data = f.read()
    finally:
        f.close() 
    zpkg = zlib.compress(source_data)
    epkg = encrypte(zpkg)
    f = open(path, "wb+")
    f.write(epkg)
    f.close()

def md5_file(fileName) :
    if not os.path.exists(fileName) :
        print("md5 file not exist", fileName)
        return ""
    m = md5()
    f = open(fileName, "rb")
    m.update(f.read())
    f.close()
    sm = m.hexdigest()
    return sm

all_chg = False
file = "tools/un_resource.proto"
sm = md5_file(file)
if file not in md5_config or md5_config[file] != sm :
    md5_dconfig[file] = sm
    all_chg = True
    
def check_dup_key(sData) :
    if not ("check_dup_key" in config and config["check_dup_key"]) :
        return 
    keys = []
    for entry in sData :
        keys.append(entry.ID)
    if len(keys) == 0 :
        return
    key = keys[0]
    id_field = 0
    for field in sData._message_descriptor.fields :
        if field.name == "ID" :
            id_field = field
    if id_field.type == id_field.TYPE_MESSAGE :
        for sfield in id_field.message_type.fields :
            print(sfield.name)
    def key_sort(a, b) :
        if id_field.type == id_field.TYPE_MESSAGE :
            for sfield in id_field.message_type.fields :
                sfname = sfield.name
                if getattr(a,sfname) < getattr(b,sfname) :
                    return -1 
                elif getattr(a,sfname) > getattr(b,sfname) :
                    return 1
            assert False, "duplicate key:"+str(a)
        else :
            if a > b :
                return 1
            elif a < b :
                return -1 
            else :
                assert a != b, "duplicate key:"+str(a)
    keys.sort(key_sort)

def transData() :
    for tconfig in config['excels'] :
        tClass = getattr(proto, tconfig['struct'])
        tData = tClass()

        is_chg = all_chg
        for file in tconfig['file'] :
            sm = md5_file(file)
            if file not in md5_config or md5_config[file] != sm :
                md5_dconfig[file] = sm
                is_chg = True
        
        if "resourceDir" in localConfig : 
            toPb = True
            if "toLua" in tconfig and tconfig["toLua"] :
                toPb = False
                file = tconfig["struct"] + ".lbytes"
                sm = md5_file(localConfig["resourceDir"] + file)
                if file not in md5_config or md5_config[file] != sm :
                    is_chg = True
            elif "toJson" in tconfig and tconfig["toJson"] :
                toPb = False
                file = tconfig["struct"] + ".json"
                sm = md5_file(localConfig["resourceDir"] + file)
                if file not in md5_config or md5_config[file] != sm :
                    is_chg = True
            if toPb or ("toPb" in tconfig and tconfig["toPb"]) :
                file = tconfig["struct"] + ".bytes"
                sm = md5_file(localConfig["resourceDir"] + file)
                if file not in md5_config or md5_config[file] != sm :
                    is_chg = True
        if "jsonDir" in localConfig :
            file = tconfig["struct"] + ".json"
            sm = md5_file(localConfig["jsonDir"] + file)
            if file+"S" not in md5_config or md5_config[file+"S"] != sm :
                    is_chg = True            
        
        if not is_chg :
            continue
        sData = tData.list
        for file in tconfig["file"] : 
            print("trans:", file, tconfig['struct'].encode("utf-8"))
            #print("trans:", file.encode("UTF-8"), tconfig['struct'].decode("UTF-8").encode("GBK"))
            book = xlrd.open_workbook(EXCEL_DIR+"/"+file, formatting_info=True)
            sheets = book.sheets()
            for index, sheet in enumerate(sheets):
                if sheet.name in config["exclude"]:
                    continue
                if sheet.nrows <= tconfig['titleCol'] or sheet.nrows <= tconfig['titleCol'] :
                    continue
                    
                print("trans sheet\t" + sheet.name.encode("gb2312"))

                firstRows = sheet.row(tconfig['titleCol'])
                debug_log(firstRows)
                firstRow = []
                for col in firstRows : 
                    firstRow.append(str(col.value))

                desc = genDesc("", firstRow, sData._message_descriptor.fields, True) 
                #debug_log(desc)

                for i in range(tconfig['startCol'], sheet.nrows) : 
                    row = sheet.row(i)
                    if str(row[0].value) == '' : 
                        continue
                    eData = sData.add()
                    try :
                        setValue(eData, row, desc, tconfig)
                    except Exception,e:
                        print("set value error:"+":"+file+":"+tconfig['struct'] + ":" + sheet.name+":"+str(i)+":"+str(row[0].value))
                        print Exception,":",e
                        raise Exception
    
        check_dup_key(sData)

        if "resourceDir" in localConfig : 
            toPb = True
            if "toLua" in tconfig and tconfig["toLua"] :
                toPb = False
                file = tconfig["struct"] + ".lbytes"
                dumpLua(tData, localConfig["resourceDir"] + file)
                md5_dconfig[file] = md5_file(localConfig["resourceDir"] + file)
            elif "toJson" in tconfig and tconfig["toJson"] :
                toPb = False
                file = tconfig["struct"] + ".json"
                dumpJson(tData, localConfig["resourceDir"] + file)
                md5_dconfig[file] = md5_file(localConfig["resourceDir"] + file)
            if toPb or ("toPb" in tconfig and tconfig["toPb"]) :
                file = tconfig["struct"] + ".bytes"
                dumpPB(tData, localConfig["resourceDir"] + file) 
                md5_dconfig[file] = md5_file(localConfig["resourceDir"] + file)

        if "jsonDir" in localConfig :
            try :
                os.makedirs(localConfig["jsonDir"])
            except OSError, why :
                i = 1
            file = tconfig["struct"] + ".json"
            dumpJson(tData, localConfig["jsonDir"] + file)
            md5_dconfig[file+"S"] = md5_file(localConfig["jsonDir"]+file)

    if "serverDir" in localConfig :
        shutil.copy("tools/MapData.proto", localConfig["serverDir"] + SERVER_DIR + "/src/")
        if "mapDir" in localConfig : 
            os.system("svn up " + localConfig["mapDir"])
            if os.path.exists(localConfig["serverDir"] + SERVER_DIR + "/xgame_resource/MapData") :
                shutil.rmtree(localConfig["serverDir"] + SERVER_DIR + "/xgame_resource/MapData", True)
            shutil.copytree(localConfig["mapDir"], localConfig["serverDir"] + SERVER_DIR + "/xgame_resource/MapData")



def transCS_(name) :
    os.system("tools\dep\ProtoGen\protogen -i:tools\%s.proto -o:%s\%s.cs"%(name, localConfig["csDir"], name))

def transCS() :
    debug_log(localConfig)
    if "serverDir" in localConfig  :
        shutil.copy(localConfig["serverDir"] + SERVER_DIR + "src/un.proto", SCRIPT_DIR)
        shutil.copy(localConfig["serverDir"] + SERVER_DIR + "src/un_dir.proto", SCRIPT_DIR)
        shutil.copy(localConfig["serverDir"] + "frame/src/cs/login/" + "src/login.proto", SCRIPT_DIR)
    
    if "csDir" in localConfig :
        transCS_("un_resource")
 #       transCS_("login")
        transCS_("un")
 #       transCS_("MapData")
 #       transCS_("ClientData")
 #       transCS_("un_dir")
 #       transCS_("CAssetBundleData")
        
transCS()
transData()
save_json(EXCEL_DIR + "/md5_config.json", md5_dconfig)
        

#
#f = open(localConfig["resourceDir"] + "ResHero.bytes", "rb")
#pkg = f.read()
#debug_log(len(pkg))
#d = proto.ResHero()
#d.ParseFromString(pkg)
#debug_log(d)

