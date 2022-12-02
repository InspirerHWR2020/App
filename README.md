# INSPIRER: Object Placement Demo

## Inhalt
1. [Backend](#backend)
   1. [Installation](#installation)
   2. [Konfiguration](#konfiguration)
   3. [Backend Struktur](#backend-struktur)
   4. [API](#api)
2. [Frontend](#frontend)
   1. [Anbindung an Backend](#anbindung-an-backend)
   2. [UI](#ui)

----

## Backend
Das Backend besteht aus vier Docker Containern, die auf einem Hostserver laufen.

### Installation
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


### Konfiguration
#### `docker-compose.yaml`

#### pgAdmin 4

#### nginx

#### AssetBundles und glTF/GLB Dateien



### Backend Struktur
<details><summary>Abbildung</summary>
<img src="https://user-images.githubusercontent.com/88034713/205385541-4a3bf0ab-c08c-462a-9173-cc66036ef0fd.jpg">
</details>


### API


----
## Frontend

### Anbindung an Backend

### UI


----
## Mitwirkende
@KuroKurama01
@Luc1412
@nbethmann
@paulchen63
@Hutmensch
> Und jetzt... Affengeräusche. :monkey:
