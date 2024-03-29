# TcpListenerAsNTService

Le détail est ici :

Le code du service NT est dans WindowsService.
Le nom d’affichage de ce service NT est TcpListenerAsNTService.

Il peut se lancer en console ou en service Windows en utilisant les commandes suivantes depuis le répertoire binaire de compilation.
- `WindowsService.exe /Install`
- `WindowsService /Start` pour démarrer ou directement dans services.msc.

Le client est dans ClientConsole : il suffit d’appuyer sur une touche.

Pour arrêter le service NT, simplement Stop de services.msc ou `WindowsService.exe /Stop`


 

