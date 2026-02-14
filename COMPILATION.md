# 🔨 GUIDE DE COMPILATION - Créer le .exe

## ⚙️ PRÉREQUIS

- Visual Studio 2022 (Community gratuit)
- .NET 8.0+ SDK
- PowerShell (Windows)

---

## 🚀 COMPILATION RAPIDE

### Étape 1: Récupérer les fichiers source

Les fichiers se trouvent dans le dossier `MinecraftStatusAgent`:
```
MinecraftStatusAgent/
├── source/
│   ├── MinecraftStatusAgent.csproj
│   └── Program.cs
└── README.md
```

### Étape 2: Ouvrir PowerShell (Admin)

```powershell
cd C:\Users\VotreNomUtilisateur\Documents\MinecraftStatusAgent\source
```

### Étape 3: Compiler en Release

```powershell
# Compiler (crée le .exe)
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true

# Ou plus simple (sans self-contained):
dotnet publish -c Release -r win-x64
```

### Étape 4: Récupérer le .exe

Le fichier .exe sera ici:
```
bin\Release\net8.0-windows\win-x64\publish\MinecraftStatusAgent.exe
```

Copiez-le dans un dossier accessible.

---

## 📦 DÉTAILS DE COMPILATION

### Build Debug (pour développer)
```powershell
dotnet build -c Debug
# Fichier: bin\Debug\net8.0-windows\MinecraftStatusAgent.exe
```

### Build Release (pour distribuer)
```powershell
dotnet publish -c Release -r win-x64 --self-contained
# Plus gros (150 MB) mais ne nécessite pas .NET installé
```

### Build Release (sans self-contained)
```powershell
dotnet publish -c Release -r win-x64
# Plus petit (5 MB) mais nécessite .NET 8.0 installé
```

---

## ✅ VÉRIFIER QUE ÇA COMPILE

Vous devez voir:
```
Building for platform: win-x64
...
MSBuild version...
Build succeeded.
✅ Publish succeeded
```

---

## 🎯 RÉSULTAT

Vous avez maintenant:
```
MinecraftStatusAgent.exe (5-150 MB selon compilation)
```

Ce fichier est **prêt à distribuer**!

---

## 📝 COMMANDES COMPLÈTES À COPIER-COLLER

### Pour les débutants:
```powershell
cd Documents\MinecraftStatusAgent\source
dotnet publish -c Release -r win-x64
```

Le fichier sera dans: `bin\Release\net8.0-windows\win-x64\publish\MinecraftStatusAgent.exe`

---

**Voilà! Vous avez votre .exe ! 🎉**
