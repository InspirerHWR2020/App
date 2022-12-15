# INSPIRER: Object Placement Demo

https://user-images.githubusercontent.com/88034713/206235447-e629eb81-8ec8-4f6f-be75-99d68800bebd.mp4

# Inhalt
1. [Backend](#backend)
   1. [Struktur](#struktur)
   2. [API](#api)
   3. [Installation](#installation)
   4. [Konfiguration](#konfiguration)
2. [Frontend](#frontend)
   1. [Konfiguration](#konfiguration-1)
   2. [Installation](#installation-1)
   3. [Skripte](#skripte)
   4. [UI](#ui)

-----------------------------------------------------------------------------------

# Backend


## Struktur
Das Backend besteht aus vier Docker Containern, die auf einem Hostserver laufen.

<details><summary>Abbildung</summary>

   ![](/docs/images/backend_structure.jpg)
   
</details>

- database
  - PostgreSQL-Datenbank
  - Speicherung der Daten der platzierbaren Objekte
  - Enthält Namen der Objekte und zusätzliche Informationen, die zum richtigen Abrufen und Platzieren benötigt werden.
- pgadmin
  - pgAdmin 4
  - Administrationsoberfläche für die im `database`-Container laufende Datenbank
  - Dient dem manuellen Hinzufügen von Objekteinträgen.
  - Die Daten werden im Verzeichnis `/data` ([Link](/Backend/data)) gespeichert
- postgrest
  - RESTful API für die im `database`-Container laufende Datenbank
  - Dient dem Abrufen von Informationen von der Datenbank durch die AR-App
- nginx
  - Webserver, Reverse Proxy
  - Wird mithilfe der Datei `nginx.conf` ([Link](/Backend/nginx.conf)) konfiguriert
  - Dient als universelle Schnittstelle für Anfragen aus der AR-App
  - Stellt die im Verzeichnis `assetbundles` ([Link](/Backend/assetbundles)) abgelegten, zu den platzierbaren Objekten gehörigen AssetBundles, glTF-/GLB-Dateien und Texturen unter `http://<server>:8080/bundles` zur Verfügung
  - Leitet Anfragen an `http://<server>:8080/database` an den `postgrest` Container weiter


## API

### `/database`
Alle Anfragen an `http://<server>:8080/database` werden an die PostgREST API weitergeleitet. Weitere Informationen können in der entsprechenden [Dokumentation](https://postgrest.org/en/stable/api.html) gefunden werden.

#### `http://<server>:8080/database/bundles`
<details><summary>Als Ergebnis erhält man eine Liste aller platzierbaren Objekte in der Tabelle `bundles` inklusive der zugehörigen Informationen im JSON-Format:</summary>
   <p>
      
      [
          {
              "id": 1,
              "file_name": "streetlampone",
              "display_name": "Straßenlaterne (klein)",
              "asset_name": "StreetLamp1_Short (Concrete)",
              "gltf": false,
              "additional_files": null,
              "custom_rotation_x": 0,
              "custom_rotation_y": 0,
              "custom_rotation_z": 0
          },
          {
              "id": 2,
              "file_name": "streetlampbigdouble",
              "display_name": "Straßenlaterne (groß, doppelseitig)",
              "asset_name": "StreetLamp1_TallDouble (Concrete)",
              "gltf": false,
              "additional_files": null,
              "custom_rotation_x": 0,
              "custom_rotation_y": 0,
              "custom_rotation_z": 0
          },
          {
              "id": 4,
              "file_name": "Simple_Wooden_Dining_Chair.glb",
              "display_name": "Holzstuhl",
              "asset_name": null,
              "gltf": true,
              "additional_files": [
                  "Poplar_4K_Roughness.jpg",
                  "Poplar_4K_Albedo.jpg",
                  "Poplar_4K_Normal.jpg"
              ],
              "custom_rotation_x": 0,
              "custom_rotation_y": 0,
              "custom_rotation_z": 0
          },
          {
              "id": 3,
              "file_name": "sink_mixer.gltf",
              "display_name": "Wasserhahn",
              "asset_name": null,
              "gltf": true,
              "additional_files": [
                  "sink-mixer-ao.png"
              ],
              "custom_rotation_x": 90,
              "custom_rotation_y": 0,
              "custom_rotation_z": 0
          }
      ]
      
   </p>
</details>

#### `http://<server>:8080/database/bundles?id=eq.1`
<details><summary>Als Ergebnis erhält man eine Liste aller platzierbaren Objekte in der Tabelle `bundles` mit der ID `1` inklusive der zugehörigen Informationen im JSON-Format:</summary>
   <p>
      
      [
          {
              "id": 1,
              "file_name": "streetlampone",
              "display_name": "Straßenlaterne (klein)",
              "asset_name": "StreetLamp1_Short (Concrete)",
              "gltf": false,
              "additional_files": null,
              "custom_rotation_x": 0,
              "custom_rotation_y": 0,
              "custom_rotation_z": 0
          }
      ]
      
   </p>
</details>


### `/bundles`
Die Anfragen an `/bundles` werden nicht weitergeleitet und stellen Dateien aus dem Verzeichnis `assetbundles` ([Link](/Backend/assetbundles)) zur Verfügung. Wird keine Datei sondern ein Verzeichnis angefragt, wird als Antwort eine automatisch generierte HTML-Datei gesendet, die den Inhalt des Verzeichnisses darstellt.

#### `http://<server>:8080/bundles/`
<details><summary>Als Ergebnis erhält man eine HTML-Datei, die den Inhalt des angefragten Verzeichnisses enthält.</summary>
   <p>
      
      <html>
         <head>
            <title>Index of /bundles/</title>
         </head>
         <body>
            <h1>Index of /bundles/</h1>
            <hr>
            <pre><a href="../">../</a>
            <a href="Simple_Wooden_Dining_Chair/">Simple_Wooden_Dining_Chair/</a>                        02-Dec-2022 10:01                   -
            <a href="sink_mixer/">sink_mixer/</a>                                        02-Dec-2022 10:01                   -
            <a href="Simple_Wooden_Dining_Chair.glb">Simple_Wooden_Dining_Chair.glb</a>                     01-Dec-2022 16:35            17836224
            <a href="sink_mixer.gltf">sink_mixer.gltf</a>                                    30-Nov-2022 23:49            10915125
            <a href="streetlampbigdouble">streetlampbigdouble</a>                                28-Nov-2022 21:39             3630762
            <a href="streetlampone">streetlampone</a>                                      27-Nov-2022 23:49             3589969
            </pre>
            <hr>
         </body>
      </html>
      
   </p>
   
   ![](/docs/images/bundles_html.png)
   
</details>

#### `http://<server>:8080/bundles/streetlampone`
Als Ergebnis erhält man das AssetBundle `streetlampone` in Dateiform.


## Installation
Zur Nutzung der Container müssen [Docker](https://www.docker.com/) und [Docker Compose](https://docs.docker.com/compose/install/) auf dem Host installiert sein. Außerdem werden folgende Docker Images benötigt:
- `dpage/pgadmin4` (getestet auf Version 6.17)
- `nginx` (getestet auf Version 1.23.2)
- `postgres` (getestet auf Version 15.1)
- `postgrest/postgrest` (getestet auf Version 10.1.1)

Vor der Erstellung der Container müssen die beigelegten Dateien `docker-compose.yaml` ([Link](/Backend/docker-compose.yaml)) und `nginx.conf` ([Link](/Backend/nginx.conf)) in einem Verzeichnis mit folgender Struktur abgelegt werden:
```
├─ assetbundles
│  └─ ...
├─ data
│  └─ ...
├─ docker-compose.yaml
└─ nginx.conf
```
Eine vorbefüllte Beispielstruktur ist im Verzeichnis [Backend](/Backend) vorhanden. Die Nutzung der hier enthaltenen Dateien erleichtert die spätere Konfiguration.

Die Container können nun gebaut, erstellt und gestartet werden, indem der Befehl `docker compose up` im soeben erstellten Verzeichnis ausgeführt wird. ([Dokumentation](https://docs.docker.com/engine/reference/commandline/compose_up/))

Getestet werden kann die Verbindung nun unter `http://<host-ip>:8080/test`. Bei erfolgreicher Einrichtung sollte `200 OK` zurückgegeben werden.
Die Administrationsseite der Datenbank (pgAdmin 4) sollte unter `http://<host-ip>:8090` erreichbar sein.


## Konfiguration
Bei Anpassung der Serveradressen und Ports bitte die Frontend-Konfiguration wie im [entsprechenden Abschnitt](#konfiguration-1) beschrieben anpassen.

### Freigelegte Ports
Die freigelegten Ports der NGINX- und des pgAdmin-Containers können angepasst werden, indem in der Datei `docker-compose.yaml` ([Link](/Backend/docker-compose.yaml)) die erste Zahl des `ports`-Parameters geändert wird. Anschließend kann der Befehl `docker-compose up -d` ausgeführt werden, um die angepassten Container erneut zu erstellen.

### pgAdmin 4
#### Standarduser erstetzen
Vor der Nutzung sollte der Standard-User für pgAdmin deaktiviert und ein neuer User erstellt werden. Dazu geht man folgendermaßen vor:
1. Man ruft die pgAdmin-Seite des Containers auf und meldet sich mit den anfangs gesetzten Anmeldedaten an: 
   - Username: `test@mail.de`
   - Passwort: `banana`
2. Man öffnet das User-Menü durch klicken auf den Username in der rechten oberen Ecke (1.) und Auswahl von "Users" (2.):
   <details><summary>Bild</summary>
   
      ![](/docs/images/pgAdmin_config_1.png)
      
   </details>
3. Daraufhin muss eine Zeile hinzugefügt werden (1.) und dem User eine Email-Adresse (2.), die Adminrolle (3.) und ein Passwort (4.) zugewiesen werden. Schlussendlich werden die Änderungen gespeichert.
   <details><summary>Bild</summary>
   
      ![](/docs/images/pgAdmin_config_2.png)
      
   </details>
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
Die Standardkonfiguration des Webservers im NGINX Container ist in der Datei `nginx.conf` ([Link](/Backend/nginx.conf)) festgelegt. Hier können die Unterverzeichnisse und der Port, unter denen die Assets und die Postgrest API erreichbar sind, geändert werden. **Achtung**: bei Änderung des Ports, muss der Port des Docker-Containers ebenfalls angepasst werden. Weitere Informationen zur Konfiguration sind in der [NGINX-Dokumentation](https://www.nginx.com/resources/wiki/start/) zu finden.

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
Die App greift nur auf Dateien zu, die auch in der Tabelle `bundles` der Datenbank eingetragen sind. Diese Tabelle muss gegebenenfalls noch erstellt werden, wenn nicht, wie im Abschnitt [Installation](#installation) beschrieben, die vorhandenen Daten übernommen wurden. Pro platzierbarem Objekt wird ein Eintrag benötigt. Dabei ist die Struktur der Tabelle folgende:

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


---------------------------------------------------------------------------------------
# Frontend
Das Frontend ist eine mithilfe von Unity entwickelte AR-App für Smartphones.

## Konfiguration
Zur Anbindung an das Backend müssen die Serveradresse und der Basisphad für die PostgREST API und die Objekt-Dateien konfiguriert werden. Diese Konfigurationen befinden sich in der Datei [`Assets/Scripts/Settings.cs`](Assets/Scripts/Settings.cs). Die Konfigurationen können wie folgt gesetzt werden:

```cs
public static class Settings
{
    public static readonly string backendDatabaseUrl = "http://<server>:<port>/database";
    public static readonly string backendAssetsUrl = "http://<server>:<port>/bundles";
}
```

## Installation
### Entwicklungsumgebung
1. Zur Entwicklung wurde die Unity Version `2021.3.1f1` genutzt. Diese Version ist im [Unity Download Archiv](https://unity.com/releases/editor/archive) zu finden.
2. Das Repository kann mithilfe von git mit dem Befehl `git clone https://github.com/InspirerHWR2020/App.git` oder mithilfe von GitHub Desktop geklont werden.
3. Ist das Repository geklont, kann man es mithilfe von Unity öffnen.

### Smartphone
Es gibt zwei Wege, die App auf einem Smartphone zu installieren:
- Installation auf einem Android-Gerät mithilfe der APK-Datei
  - APK-Datei auf das Smartphone laden.
  - Installation von Apps mithilfe von lokalen APK-Dateien erlauben.
  - APK-Datei öffnen und die App installieren.
- Die App selbst mit Unity bauen und auf dein Smartphone laden.
  - Entwicklermodus auf dem Smartphone aktivieren
  - USB-Debugging auf dem Smartphone aktivieren.
  - Das Smartphone per USB mit dem Enwicklung-PC verbinden und dem USB-Debugging über diesen PC zustimmen.
  - In Unity `File` und `Build and Run` auswählen.


## Skripte
Die bedeutendsten Skripte sind [`MainScene.cs`](/Assets/Scripts/MainScene.cs) und [`BackendConnection.cs`](/Assets/Scripts/BackendConnection.cs). Die Methoden und Variablen dieser Klassen sind im Code ausführlich kommentiert, sodass hier nicht sehr stark darauf eingegangen wird.

### MainScene.cs
Die gleichnamige Klasse erbt von MonoBehaviour und wird als eine Art Hauptskript der Szene (`MainScene`) genutzt. Sie ermöglicht das Setzen von Objekten, die Anzeige des "Fadenkreuzes" und das Suchen von Objekten mithilfe der Suchleiste. Außerdem nutzt sie Methoden aus `BackendConnection.cs`, um platzierbare Objekte zu laden.

#### Öffentliche Variablen
- `objectToPlace`
  - GameObject, das mithilfe der Methode `PlaceObject` platziert werden kann
- `placementIndicator`
  - GameObject, welches als "Fadenkreuz" genutzt wird, um anzuzeigen, wo das nächste Objekt platziert wird
- `listObjectPrefab`
  - GameObject, das ein Objekt in der Auswahlliste darstellt


### BackendConnection.cs
Die gleichnamige Klasse dient als Verbindung zum Backend und ermöglicht die Abfrage aller Informationen zu platzierbaren Objekten aus dem Backend von der Datenbank sowie aller zu einem Objekt gehörigen Dateien. Außerdem stellt sie mit `BundleInfo` eine Klasse zur Verfügung, die dem Speichern eines Eintrages in die [Tabelle `bundles`](#datenbankeintrag-erg%C3%A4nzen) und dem dazugehörigen GameObject dient.


## UI
Jegliche Objekte und Elemente des User Interface werden per Hirarchiestruktur angeordnet. Um in Unity ein Interface erstellen zu können, gilt es zuerst einen Hauptcanvas zu erstellen, der die restlichen UI Elemente beinhaltet und als Hauptbildschirm für die Applikation fungiert. Der Hauptcanvas, `User InterfaceCanvas` genannt, ist hierbei der oberste Knotenpunkt in der Hirarchie des User Interface. Da bei der Applikation das Bild hauptsächlich von der Kamera durch `AR Session` bezogen wird, wird der Hauptcanvas bis auf die nötigen interagierbaren Elemente leer gelassen. Für das User Interface sind bisher 4 weitere Ansichten implementiert worden, wovon 3 zur Laufzeit in Benutzung sind.
- `MenuButtons`
   - Buttons zur Navigation durch die Applikation
- `ObjectMenuCanvas`
   - Menü zur Auswahl eines zu platzierenden Objektes
- `SideMenu`
   - Seitenmenü für weitere mögliche Interaktionsmöglichkeiten / Einstellungen
- `PlacedObjectInfo`
   - Fenster zum Anzeigen der Informationen eines platzierten Objekts
   - bisher nicht in Verwendung

### MenuButtons
In diesem Bereich befinden sich die Buttons, die zur Navigations in die verschiedenen Fenster der Applikation von Nöten sind. Gemäß des Entwurfs gibt es in diesem Bereich 3 Buttons die zum einen in das Seitenmenü führen sollen, das Fenster zur Objektauswahl ausklappen sollen und ein weiterer Button der für verschiedene Zwecke benutzt werden kann. Die verschiedenen Buttons funktionieren hierbei mit Eventhandlern, die eine entsprechende Animation abspielen sobald einer der Buttons geklickt wird. So spielt der `SideMenuButton` eine entsprechende [Animation] die das Seitenmenü ausfährt, wenn Dieser angeklickt wird. Nach dem gleichen Prinzip spielt der `AddButton` eine [Animation](#animation) ab, die das Objektmenü ausfährt.

### ObjectMenuCanvas
In diesem Fenster befindet sich die Ansicht zum Auswählen von zu platzierenden Objekten. Hierfür wurde ein Canvas benutzt, da beim Ausblenden eines Canvas die Inhalte nicht extra geladen werden, was zu einer besseren Performance führt. In diesem Fenster befindet sich zuerst ein Bereich `CloseArea` der zur Schließung des Fensters als Button fungiert und die Animation zum ausfahren des Objektmenüs rückwärts abspielt, wodurch sich das Fenster wieder schließt. Unterhalb des `CloseArea` Bereichs befindet sich eine Suchleiste `SearchImageInput` mit der man gelistete Objekte filtern kann. Desweiteren befindet sich ein Bereich zur Auswahl einer Kategorie, zur weiteren Filterung der gelisteten Ergebnisse, namens `CategoriesButtons`. Ziel hierbei ist es, Kategorien dynamisch aus der Datenbank zu laden, und mittels Prefabs in den Bereich hinzuzufügen. Dieser Abschnitt ist bisher jedoch noch nicht implementiert. Zuletzt gibt es noch einen weiteren Bereich `ObjectsCanvas`, der zum Anzeigen der platzierbaren Objekte benutzt wird. In diesem Bereich werden die Objekte, je nach Filterung, dynamisch aus der Datenbank geladen und mittels Prefab in die Bereich hinzugefügt. Die Prefabs die dem Bereich hierbei hinzugefügt werden, fungieren dabei als Button. Diese haben per Script einen Eventhandler implementiert, der das jeweilige ausgewählte Model eines Objekts lädt, und das Objektmenü schließt.

### SideMenu
In dem Seitenmenü sind bisher noch keine weiteren Funktionen implementiert, bis auf den Button `SideMenuButton` der das Fenster wieder schließt. Es sind lediglich Platzhalter für Menüoptionen vorhanden, die in der Theorie auch dynamisch in den Bereich geladen werden können. Hierfür ist jedoch noch keine weitere Vorarbeit geleistet worden.

### PlacedObjectInfo
Da dieses Fenster bisher noch nicht in Nutzung zur Laufzeit ist, sind in diesem Fenster bisher nur Elemente ohne Funktion implementiert.

### Animation
Die Animationen die zum ausfahren der jeweiligen Fenster benutzt werden, werden in einer weiteren Datei gespeichert, die die Keyframes der Animation beinhaltet. Anhand der Keyframes wird dann eine Animation erstellt. Diese Datei kann dann im Anschluss z.B. einem Eventhandler übergeben werden, wodurch die Animation abgespielt wird, sollte der Fall des Eventhandlers eintreten.

### Layout und Responsiveness
Um die Applikation auf möglichst vielen Geräten mit unterschiedlichen Auflösungen benutzbar zu machen, gilt es die verschiedenen UI-Elemente demenstprechend anzupassen. Hierbei kommen Anker und Layouts zum Einsatz. Ein Anker ist hierbei der Punkt an dem ein Objekt sich an sein Vaterobjekt orientiert. So wird zum Beispiel der Bereich `MenuButtons` an die untere Seite des Hauptcanvas `User InterfaceCanvas` geankert, wodurch der Bereich auch bei unterschiedlichen Auflösungen immer unten am Bildschirm dargestellt wird. Des weiteren können Anker auch mit der Größe einer Seite skalieren. So wird z.B. das `SideMenu` der Höhe des Hauptcanvas angepasst und füllt dementsprechend auch bei unterschiedlichen Auflösungen die Höhe des Bildschirms. Ähnlich ist dem der Fall auch `ObjectMenuCanvas`, bloß dass hier der Anker sich an die untere Seite des Hauptcanvas anpasst und somit mit die in die Breite skaliert. Damit Objekte konstant mit gleichen Abständen angezeigt werden, kommen Layouts zum Einsatz. Layouts sind eine automatische Anpassung der Objekte innerhalb eines Bereiches, der ein entsprechendes Layout verwendet. So verwenden zum Beispiel die Fenster `SideMenu` und `MenuButton` beide ein Layout, um die Objekte innerhalb richtig darzustellen. `SideMenu` verwendet ein Vertical Layout, wodurch die Objekte innerhalb des Fensters vertikal angeordnet werden. `MenuButton` hingegen verwendet hingegen ein Horizontal Layout. Auch die Unterbereiche von `ObjectMenuCanvas` verwenden teilweise Layouts, um das dynamische Laden der Objekte möglichst problemfrei zu gestalten. `CategoriesButtons` verwendet hierbei ein Horizontal Layout und `ObjectsCanvas` ein Vertical Layout. Trotz der umgesetzen Technicken gibt es bei manchen Auflösung dennoch Probleme, da Unity's Responsiveness Möglichkeiten teilweise sehr unintuitiv sind.

----
# Mitwirkende
[@KuroKurama01](https://github.com/KuroKurama01)
[@Luc1412](https://github.com/Luc1412)
[@nbethmann](https://github.com/nbethmann)
[@paulchen63](https://github.com/paulchen63)
[@Hutmensch](https://github.com/Hutmensch)
> Und jetzt... Affengeräusche. :monkey:
