proc HelloNim*(a: cint): cstring {.cdecl, exportc, dynlib.} =
  return "bob"

proc HelloDim*(a: cint): cint {.cdecl, exportc, dynlib, discardable.} =
  echo "bob"
  return