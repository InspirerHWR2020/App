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
Die freigelegten Ports der NGINX- und des pgAdmin-Containers können angepasst werden, indem in der Datei `docker-compose.yaml` die erste Zahl des `ports`-Parameters geändert wird. Anschließend kann der Befehl `docker-compose up -d` ausgeführt werden, um die angepassten Container erneut zu erstellen.

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
1. In der Menüleiste am oberen Rand "Object" > "Register" > "Server..." anwählen.
2. Einen beliebigen Servernamen eingeben.
3. Die Registerkarte "Conncection" auswählen und folgende Standarddaten einfüllen:
   - Hostname: `database`
   - Port: `5432`
   - Maintenance database: `postgres`
   - Username: `postgres`
   - Password: `banana`

### nginx
Die Standardkonfiguration des Webservers im NGINX Container ist in der Datei `nginx.conf` festgelegt. Hier können die Unterverzeichnisse und der Port, unter denen die Assets und die Postgrest API erreichbar sind, geändert werden. **Achtung**: bei Änderung des Ports, muss der Port des Docker-Containers ebenfalls angepasst werden. Weitere Informationen zur Konfiguration sind in der [NGINX-Dokumentation](https://www.nginx.com/resources/wiki/start/) zu finden.

### AssetBundles und glTF/GLB Dateien
Um weitere Objekte in der AR-App zur Verfügung stellen zu können, müssen die entsprechenden Ressourcen im richtigen Format im Ordner `assetbundles` abgelegt und zur Datenbank hinzugefügt werden. Dazu muss man bei AssetBundles und glTF- bzw. GLB-Dateien verschieden vorgehen.

#### Ablegen im `assetbundles` Ordner

**AssetBundles** müssen lediglich im Ordner `assetbundles` abgelegt werden. Im AssetBundle müssen ein Prefab für das Objekt und die dazugehörigen Assets vorhanden sein.

Zur Verwendung von **glTF- und GLB-Dateien** werden diese ebenfalls im Ordner `assetbundles` abgelegt. Zusätzlich müssen jedoch die zugehörigen Texturen in einem Ordner abgelegt werden, dessen Name dem der Datei ohne die Dateiendung entspricht.

<details><summary>Beispiel: Ordnerstruktur</summary>
   <p>
      Diese Ordnerstruktur enthält `streetlampone` als Beispiel für ein AssetBundle und `sink_mixer.gltf` mit der zugehörigen Textur als Beispiel für eine glTF-Datei.
      
      ├─ assetbundles
      │  ├─ sink_mixer
      │  │  └─ sink-mixer-ao.png
      │  ├─ sink_mixer.gltf
      │  ├─ steetlampone
      │  └─ ...
      ├─ data
      │  └─ ...
      ├─ docker-compose.yaml
      └─ nginx.conf
      
   </p>
</details>


#### Datenbankeintrag ergänzen
Die App greift nur auf Dateien zu, die auch in der Tabelle `bundles` der Datenbank eingetragen sind. Pro platzierbarem Objekt wird ein Eintrag benötigt. Dabei ist die Struktur der Tabelle folgende:

| Spalte             | Datentyp  | Not NULL  | Standardwert | Beschreibung                                                             |zu setzen für |
|--------------------|-----------|-----------|--------------|--------------------------------------------------------------------------|--------------|
|id                  |integer    |wahr       |laufende Zahl |automatisch generierter Primärschlüssel                                   |alle          |
|file_name           |text       |wahr       |null          |Name der direkt in `assetbundles` abgelegten Datei                        |alle          |
|display_name        |text       |falsch     |null          |Anzeigename für die UI                                                    |alle          |
|asset_name          |text       |falsch     |null          |Name des Prefabs innerhalb eines AssetBundles                             |AssetBundles  |
|gltf                |boolean    |wahr       |false         |Gibt an, ob es sich um eine glTF-/GLB-Datei oder ein AssetBundle handelt  |alle          |
|additional_files    |text[]     |falsch     |null          |Liste der im zugehörigen Ordner abgelegten Texturen für glTF-/GLB-Dateien |glTF/GLB      |
|custom_rotation_x   |integer    |wahr       |0             |Rotation des Objektes in x-Richtung, um es richtig auszurichten.          |alle          |
|custom_rotation_y   |integer    |wahr       |0             |Rotation des Objektes in y-Richtung, um es richtig auszurichten.          |alle          |
|custom_rotation_z   |integer    |wahr       |0             |Rotation des Objektes in z-Richtung, um es richtig auszurichten.          |alle          |


Diese Datenbankeinträge enthalten analog zum Ordnerpfad Beispiel im Abschnitt *[Ablegen im `assetbundles` Ordner](#ablegen-im-assetbundles-ordner)* `streetlampone` als AssetBundle und `sink_mixer.gltf` als glTF-Datei.

| id  | file_name       | display_name             | asset_name                     | gltf   | additional_files   | custom_rotation_x  | custom_rotation_y  | custom_rotation_z  |
| --- | --------------- | ------------------------ | ------------------------------ | ------ | ------------------ | ------------------ | ------------------ | ------------------ |
| 1   | streetlampone   | Straßenlaterne (klein)   | StreetLamp1_Short (Concrete)   | false  | \[null\]           | 0                  | 0                  | 0                  |
| 2   | sink_mixer.gltf | Wasserhahn               | \[null\]                       | true   | {sink-mixer-ao.png}| 90                 | 0                  | 0                  |


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
[@KuroKurama01](https://github.com/KuroKurama01)
[@Luc1412](https://github.com/Luc1412)
[@nbethmann](https://github.com/nbethmann)
[@paulchen63](https://github.com/paulchen63)
[@Hutmensch](https://github.com/Hutmensch)
> Und jetzt... Affengeräusche. :monkey:
