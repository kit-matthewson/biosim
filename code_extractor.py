import os

files = []
for (dirpath, dirnames, filenames) in os.walk("./Assets/Scripts"):
  for filename in filenames:
    if filename.split(".")[-1] == "cs":
      files.append(dirpath + "\\" + filename)

files.sort(key=lambda x: x.split("\\")[-1])

with open("code_listings.txt", "w") as output:
  for file in files:
    with open(file) as f:
      name = file.split("\\")[-1]
      if name[0] == "_":
        continue

      content = f.read()

      output.write(f"\\section{{{name}}}\n")
      output.write("\\begin{lstlisting}[language=csh]\n")
      output.write(content)
      output.write("\n\\end{lstlisting}")
      output.write("\n\n")

      lines = content.split("\n")

      print(f"Done {name}: {len(lines)} lines")
