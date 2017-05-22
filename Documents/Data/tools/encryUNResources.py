import os
import sys
import json


EXCEL_DIR = sys.argv[1]
localConfigFile = open(EXCEL_DIR + "/config.json")
localConfig = json.load(localConfigFile, encoding='UTF-8')

def GetReplace(line, replaceStr):
    #replace ReplaceStr
    temp = line.split(replaceStr)  
    temp1 = temp[0] + replaceStr + '.m_value' + temp[1]  
    return temp1

def func(filename):  
    input = open(filename)  
    lines = input.readlines()  
    input.close()  
  
    output  = open(filename,'w')  
    
    replaceIndex = 1
    replaceStr = ''
    hasDefaultValue = False
    isList = False
    UNValueClass = ''
    for line in lines:  
        if not line:  
            break  
        if ('private int' in line and replaceIndex == 1):  
            #private Encry
            isList = False
            if('(int)' in line):
                # has defaultValue
                hasDefaultValue = True
                temp1 = line.split("private int") 
                temp2 = temp1[1].split("(int)")
                replaceStr = temp2[0].split(" = ")[0]
                defaultValue = temp2[1].split(";")[0]
                temp3 = temp1[0] + 'private UNValueInt' + replaceStr + ' = new UNValueInt(' + defaultValue + ');\n'
                output.write(temp3)
            else:
                hasDefaultValue = False
                temp1 = line.split("private int")  
                temp2 = temp1[1].split(";")
                replaceStr = temp2[0]
                temp3 = temp1[0] + 'private UNValueInt' + replaceStr + ' = new UNValueInt();\n'
                output.write(temp3)
               
            # jump line 2
            replaceIndex = 2
        elif ('private float' in line and replaceIndex == 1):  
            #private Encry
            isList = False
            if('(float)' in line):
                # has defaultValue
                hasDefaultValue = True
                temp1 = line.split("private float") 
                temp2 = temp1[1].split("(float)")
                replaceStr = temp2[0].split(" = ")[0]
                defaultValue = temp2[1].split(";")[0]
                temp3 = temp1[0] + 'private UNValueFloat' + replaceStr + ' = new UNValueFloat(' + defaultValue + ');\n'
                output.write(temp3)
            else:
                hasDefaultValue = False
                temp1 = line.split("private float")  
                temp2 = temp1[1].split(";")
                replaceStr = temp2[0]
                temp3 = temp1[0] + 'private UNValueFloat' + replaceStr + ' = new UNValueFloat();\n'
                output.write(temp3)
               
            # jump line 2
            replaceIndex = 2 
        elif ('private long' in line and replaceIndex == 1):  
            #private Encry
            isList = False
            if('(long)' in line):
                # has defaultValue
                hasDefaultValue = True
                temp1 = line.split("private long") 
                temp2 = temp1[1].split("(long)")
                replaceStr = temp2[0].split(" = ")[0]
                defaultValue = temp2[1].split(";")[0]
                temp3 = temp1[0] + 'private UNValueLong' + replaceStr + ' = new UNValueLong(' + defaultValue + ');\n'
                output.write(temp3)
            else:
                hasDefaultValue = False
                temp1 = line.split("private long")  
                temp2 = temp1[1].split(";")
                replaceStr = temp2[0]
                temp3 = temp1[0] + 'private UNValueLong' + replaceStr + ' = new UNValueLong();\n'
                output.write(temp3)
               
            # jump line 2
            replaceIndex = 2
        elif ('List<int>' in line and replaceIndex == 1):  
            #private Encry
            isList = True 
            UNValueClass = 'UNValueListInt'
            hasDefaultValue = False
            temp1 = line.split(" = ")  
            replaceStr = temp1[0].split("List<int>")[1]
            output.write(line)
            # jump line 2
            replaceIndex = 2
        elif ('List<float>' in line and replaceIndex == 1):  
            #private Encry
            isList = True 
            UNValueClass = 'UNValueListFloat'
            hasDefaultValue = False
            temp1 = line.split(" = ")  
            replaceStr = temp1[0].split("List<float>")[1]
            output.write(line)
            # jump line 2
            replaceIndex = 2
        elif replaceIndex == 2:
            # jump line 3
            output.write(line)
            replaceIndex = 3
        elif replaceIndex == 3:
            output.write(line)
            # public define jump line 4
            replaceIndex = 4
        elif replaceIndex == 4:
            output.write(line)
            # { jump line 5
            replaceIndex = 5
        elif replaceIndex == 5:  
            if isList:
                output.write('      get\n      {\n')
                output.write('        if(' + replaceStr + '.Count == 0 )\n')
                output.write('          return' + replaceStr + ';\n')
                output.write('        if(' + replaceStr + 'UNValue.m_value.Count == 0 )\n')
                output.write('          ' + replaceStr + 'UNValue = new ' + UNValueClass + '(' + replaceStr + ' );\n')
                output.write('        return ' + replaceStr + 'UNValue.m_value;\n      }\n')
            elif not hasDefaultValue:
                output.write(GetReplace(line, replaceStr))
            else:
                output.write(line)
            replaceIndex = 6
        elif replaceIndex == 6: 
            if isList:
                output.write(line)
                output.write('    private ' + UNValueClass + replaceStr + 'UNValue = new ' + UNValueClass + '();\n')
                replaceIndex = 1
            else:
                output.write(GetReplace(line, replaceStr))
                if hasDefaultValue:
                    replaceIndex = 7
                else:
                    replaceIndex = 1
        elif replaceIndex == 7:
            output.write(GetReplace(line, replaceStr))
            replaceIndex = 1
        else:  
            output.write(line)    
        
    output.close()  

func(localConfig["csDir"] +  '/un_resource.cs')