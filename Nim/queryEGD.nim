import httpClient
import strutils
import json
# var client = newHttpClient()
# echo client.getContent("http://www.europeangodatabase.eu/EGD/GetPlayerDataByPIN.php?pin=12913769")
# {"retcode":"Ok","Pin_Player":"12913769","AGAID":"17693","Last_Name":"Davis","Name":"Ian",
# "Country_Code":"IE","Club":"Belf","Grade":"1d","Grade_n":"30","EGF_Placement":"797",
# "Gor":"2128","DGor":"0","Proposed_Grade":"","Tot_Tournaments":"77",
# "Last_Appearance":"E190728A","Elab_Date":"2009-04-03","Hidden_History":"0","Real_Last_Name":"Davis","Real_Name":"Ian"}

let jsonNode = parseJson("""{"retcode":"Ok","Pin_Player":"12913769","AGAID":"17693","Last_Name":"Davis","Name":"Ian","Country_Code":"IE","Club":"Belf","Grade":"1d","Grade_n":"30","EGF_Placement":"797","Gor":"2128","DGor":"0","Proposed_Grade":"","Tot_Tournaments":"77","Last_Appearance":"E190728A","Elab_Date":"2009-04-03","Hidden_History":"0","Real_Last_Name":"Davis","Real_Name":"Ian"}""")
let pin = jsonNode["Pin_Player"].getStr()
let gor = jsonNode["Gor"].getStr()
let club = jsonNode["Club"].getStr()
let ctry = jsonNode["Country_Code"].getStr()
let lNom = jsonNode["Last_Name"].getStr()
let fNom = jsonNode["Name"].getStr()
let grd = jsonNode["Grade"].getStr()
var update:string = pin & "," & lNom & " " & fNom & "," & gor & "," & ctry & "," & club & "," & grd
echo update