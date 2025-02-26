# README: ZExtractor - A .Z File Extractor


## 📌 Overview
ZExtractor is a **.NET 6 Console Application** that automatically scans a folder, detects **.Z** compressed files, extracts their contents, and prints them to the console.

This program works inside a **Docker container** on **Linux-based environments**, including **Azure Kubernetes Service (AKS)**. It uses **SharpZipLib** and **SharpCompress** to handle compressed files.

---

## 🛠 How It Works
1. The program **waits for 5 minutes** before starting (useful for debugging in containers, commented for now).
2. It **scans** the folder `/app/data/` for any files ending in `.Z`.
(Note: currrently docker build adds two files by default:
# ls
sample.tar.Z  sample.txt.Z)

4. If a **regular `.Z` file** is found, it **extracts it** and prints its content.
5. If a **`.tar.Z` file** is found, it **extracts the `.tar` first**, then extracts its contents.
6. The extracted text is **displayed on the console**.

---

## 🏗 Technologies Used
- **.NET 6 Console App**
- **SharpZipLib** (For `.Z` file decompression)
- **SharpCompress** (For `.tar` extraction)
- **Docker** (For containerization)

---

## 🚀 How to Run (Docker)
### **Step 1: Build the Docker Image**
Run this command in the project folder:
```sh
docker build -t my-dotnet-app .
```

### **Step 2: Run the Container**
```sh
docker run --rm my-dotnet-app
```

### **Step 3: Check the Output**
If files are successfully extracted, the console should show:
```sh
Processing file: /app/data/sample.txt.Z
Detected .Z file. Extracting...
Successfully decompressed: /app/data/sample.txt

Extracted File Content:
------------------------
This is a test file for .Z extraction
(Random data)
End of File
------------------------
```

---

## 📂 Folder Structure
```
/app/
  ├── data/  (Holds .Z files and extracted content)
  │    ├── sample.txt.Z
  │    ├── sample.tar.Z
  │    ├── extracted/ (Extracted files will be placed here)
  ├── ZExtractor.dll (Main app executable)
  ├── ZExtractor.pdb (Debug symbols)
  ├── ICSharpCode.SharpZipLib.dll (Library for .Z extraction)
  ├── SharpCompress.dll (Library for .tar extraction)
```

---

## 🔍 How the Code Works
### **1️⃣ Scanning for `.Z` Files**
- The app looks inside `/app/data/` for any files ending in `.Z`.
- If no `.Z` files are found, it exits with a message.

### **2️⃣ Extracting `.Z` Files**
- If a `.Z` file is found, it is **decompressed** into its original format.
- The content is **printed to the console**.

### **3️⃣ Extracting `.tar.Z` Files**
- First, `.tar.Z` is decompressed into a `.tar` archive.
- Then, files inside the `.tar` are extracted.
- If the extracted file is a text file, its content is **printed to the console**.

---

## ⚡ Example Output
If both `sample.txt.Z` and `sample.tar.Z` exist in `/app/data/`, the program will output:
```sh
Processing file: /app/data/sample.txt.Z
Detected .Z file. Extracting /app/data/sample.txt.Z...
Successfully decompressed: /app/data/sample.txt

Extracted File Content:
------------------------
This is a test file for .Z extraction
(Random base64 data)
End of File
------------------------

Processing file: /app/data/sample.tar.Z
Detected .tar.Z file. Extracting /app/data/sample.tar.Z...
Decompressed /app/data/sample.tar.Z to /app/data/sample.tar
Extracted: sample.txt

Content of sample.txt:
------------------------
This is a test file for .Z extraction
(Random base64 data)
End of File
------------------------

All extractions complete.
```

---

## ✅ FAQ
### **1️⃣ What if the `.Z` file is too small?**
If the file is very small, `compress` might **not reduce its size**, and extraction may fail. The solution is to create a larger test file.

### **2️⃣ Can this run in Kubernetes (AKS)?**
Yes! The Docker image can be pushed to a **container registry** and deployed to **Azure Kubernetes Service (AKS).**

### **3️⃣ How to manually test inside a running container?**
Run an interactive shell inside the container:
```sh
docker run --rm -it my-dotnet-app /bin/sh
```
Then navigate to the data folder:
```sh
ls -lah /app/data/
```
If needed, manually extract a file:
```sh
uncompress -c /app/data/sample.txt.Z
```

---

## 🎯 Summary
- ✅ **Scans `/app/data/` for `.Z` files**
- ✅ **Automatically extracts `.Z` and `.tar.Z` files**
- ✅ **Prints extracted file content to the console**
- ✅ **Works inside Docker and Kubernetes (AKS)**

🚀 **Now you're ready to use `ZExtractor` for `.Z` file extraction!** 🔥

