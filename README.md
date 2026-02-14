# 🎮 Minecraft Status Agent v1.0

## 📋 Description

Agent Windows léger qui détecte automatiquement si Minecraft est lancé et envoie le statut à votre serveur toutes les 10 secondes.

**Caractéristiques:**
- ✅ Détecte Minecraft (Java et Windows 10/11 app)
- ✅ Envoie heartbeat HTTP POST
- ✅ Tray icon discret
- ✅ Logs en temps réel
- ✅ < 1% CPU, < 50MB RAM
- ✅ Open source

---

## 🚀 Installation Rapide (Utilisateurs)

1. Téléchargez `MinecraftStatusAgent.exe`
2. Double-cliquez pour lancer
3. L'app se met en tray (bas droite)
4. Lancez Minecraft → Statut passe à 🟢 EN LIGNE
5. Fermez Minecraft → Statut passe à 🔴 HORS LIGNE

**C'est tout!**

---

## 🔨 Compilation (Développeurs)

Voir: `COMPILATION.md`

---

## ⚙️ Configuration

Pour modifier la configuration, éditez `Program.cs`:

```csharp
// Ligne ~30
private string SERVER_URL = "http://localhost:5000";  // URL serveur
private string USER_ID = "1";                          // Votre ID
private int CHECK_INTERVAL = 10000;                    // Millisecondes
```

Puis recompiler.

---

## 📡 API Serveur Requise

Votre serveur doit supporter:

```
POST /api/minecraft-status
Content-Type: application/json

{
  "user_id": "1",
  "status": "online",  // ou "offline"
  "timestamp": "2024-02-14T...",
  "agent_version": "1.0.0"
}

Response:
{
  "success": true,
  "message": "Status online reçu"
}
```

---

## 🐛 Troubleshooting

### Agent ne se lance pas
```
→ Vérifier .NET 8.0 installé
→ dotnet --version
```

### Minecraft non détecté
```
→ Lancer le VRAI Minecraft (pas launcher)
→ Attendre 10 secondes
```

### Erreur "Cannot connect"
```
→ Vérifier serveur tourne
→ Vérifier SERVER_URL correct
```

---

## 📞 Support

Voir les logs:
1. Clic droit sur l'icône tray
2. Sélectionner "Afficher fenêtre"
3. Les logs expliquent les erreurs

---

## 📄 Licence

MIT License - Libre d'utilisation

---

**Version**: 1.0.0
**Status**: ✅ Production Ready
**Support**: Windows 10/11+

Bon jeu! 🎮
