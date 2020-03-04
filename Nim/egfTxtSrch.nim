import strutils

echo "Querying existing egf.txt file"
let testString = "Jones UK"
let tokns = testString.split()
for ln in lines "egf.txt":
  var OK:int = tokns.len
  for tn in tokns:
    if ln.contains(tn):
      OK = OK - 1
  if OK == 0:
    echo ln