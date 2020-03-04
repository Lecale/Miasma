import strutils

var pin:string
var nom:string
var club:string
var ctry:string
var gor:string
var grd:string
var output = open("egf.txt", fmWrite)
output.writeLine("pin,name,rating,country,club,grade")
for ln in lines "allworld_lp.html":
  case ln.len:
    of 91 , 90:
      pin=substr(ln,1,8) & "," 
      nom=substr(ln,11,48) 
      gor=substr(ln,71,74)
      ctry=substr(ln,49,50)
      club=substr(ln,52,56)
      grd=substr(ln,60,62)
      output.writeLine(pin & nom.strip  & "," & gor.strip  & "," & ctry.strip  & "," & club.strip  & "," & grd.strip  & "," )
    else:
      pin = pin