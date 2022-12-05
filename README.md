# INSPIRER: Object Placement Demo

# Inhalt
1. [Backend](#backend)
   1. [Installation](#installation)
   2. [Konfiguration](#konfiguration)
   3. [Backend Struktur](#backend-struktur)
   4. [API](#api)
2. [Frontend](#frontend)
   1. [Anbindung an Backend](#anbindung-an-backend)
   2. [UI](#ui)

----

# Backend
Das Backend besteht aus vier Docker Containern, die auf einem Hostserver laufen.

## Installation
Zur Nutzung der Container müssen [Docker](https://www.docker.com/) und [Docker Compose](https://docs.docker.com/compose/install/) auf dem Host installiert sein. Außerdem werden folgende Docker Images benötigt:
- `dpage/pgadmin4` (getestet auf Version 6.17)
- `nginx` (getestet auf Version 1.23.2)
- `postgres` (getestet auf Version 15.1)
- `postgrest/postgrest` (getestet auf Version 10.1.1)

Vor der Erstellung der Container müssen die beigelegten Dateien `docker-compose.yaml` ([Link]()) und `nginx.conf` ([Link]()) in einem Verzeichnis mit folgender Struktur abgelegt werden:
```
├─ assetbundles
│  └─ ...
├─ data
│  └─ ...
├─ docker-compose.yaml
└─ nginx.conf
```

Die Container können nun gebaut, erstellt und gestartet werden, indem der Befehl `docker compose up` im soeben erstellten Verzeichnis ausgeführt wird. ([Dokumentation](https://docs.docker.com/engine/reference/commandline/compose_up/))

Getestet werden kann die Verbindung nun unter `http://<host-ip>:8080/test`. Bei erfolgreicher Einrichtung sollte `200 OK` zurückgegeben werden.
Die Administrationsseite der Datenbank (pgAdmin 4) sollte unter `http://<host-ip>:8090` erreichbar sein.


## Konfiguration
### Freigelegte Ports
Die freigelegten Ports der nginx- und des pgAdmin-Containers können angepasst werden, indem in der Datei `docker-compose.yaml` die erste Zahl des `ports`-Parameters geändert wird. Anschließend kann der Befehl `docker-compose up -d` ausgeführt werden, um die angepassten Container erneut zu erstellen.

### pgAdmin 4
#### Standarduser erstetzen
Vor der Nutzung sollte der Standard-User für pgAdmin deaktiviert und ein neuer User erstellt werden. Dazu geht man folgendermaßen vor:
1. Man ruft die pgAdmin-Seite des Containers auf und meldet sich mit den anfangs gesetzten Anmeldedaten an: 
   - Username: `test@mail.de`
   - Passwort: `banana`
2. Man öffnet das User-Menü durch klicken auf den Username in der rechten oberen Ecke (1.) und Auswahl von "Users" (2.):
   <img src="https://user-images.githubusercontent.com/88034713/205608513-346fe3cf-9a50-4a9d-8719-4bb4221fd333.png" width=50%>
3. Daraufhin muss eine Zeile hinzugefügt werden (1.) und dem User eine Email-Adresse (2.), die Adminrolle (3.) und ein Passwort (4.) zugewiesen werden. Schlussendlich werden die Änderungen gespeichert.
   <img src="https://user-images.githubusercontent.com/88034713/205610340-f4504735-d024-40a7-9095-6af1c986cfff.png" width=50%>
4. Anschließend muss man sich abmelden, Schritte 1 und 2 nun mit dem neuen Account durchführen und den Standardnutzer durch Anklicken des Mülltonnen-Icons auf der linken Seite des User-Menüs gelöscht werden.

#### Datenbank verbinden
Um die Datenbank mit der pgAdmin-Oberfläche zu verbinden, müssen folgende Schritte durchgeführt werden:
1. 


### nginx

### AssetBundles und glTF/GLB Dateien



## Backend Struktur
<details><summary>Abbildung</summary>
<img src="https://user-images.githubusercontent.com/88034713/205385541-4a3bf0ab-c08c-462a-9173-cc66036ef0fd.jpg" width=50%>
</details>


## API


----
# Frontend

## Anbindung an Backend

## UI


----
# Mitwirkende
@KuroKurama01
@Luc1412
@nbethmann
@paulchen63
@Hutmensch
> Und jetzt... Affengeräusche. :monkey:
