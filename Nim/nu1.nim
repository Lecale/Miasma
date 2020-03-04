import zip/zipfiles
{.compile: ("zlib-1.2.11/*.c", "zlib_$#.obj").}
var filename = "allworld_lp.zip"
var z: ZipArchive
if not z.open(filename):
  echo "Opening zip failed"
  quit(1)
z.extractAll("")
z.close()