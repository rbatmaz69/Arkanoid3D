# Bauklotz – Breakout aus Grundformen

Teammitglieder: <Refik Bilal Batmaz> (<215807>)

Unity Version: 6000.6.3f1 (Unity 6.6)

## Verwendete Assets

- Schrift: Unbounded (ExtraBold), Google Fonts, SIL Open Font License 1.1 – https://fonts.google.com/specimen/Unbounded
- Schrift: Archivo (ExtraBold, SemiBold), Google Fonts, SIL Open Font License 1.1 – https://fonts.google.com/specimen/Archivo
- Alle 3D-Objekte, Materialien und UI-Elemente bestehen aus Unity-Grundformen und Unity-Bordmitteln. Es werden keine weiteren Assets und Plugins verwendet.

## Links

- Drive-Ordner mit Gameplay-Video und spielbarer Version (.exe): <Drive-Link>
- Gameplay-Video: <Link zum Video>
- Spielbarer Build: <Link zur .exe bzw. zum ZIP>

## Startup

Aus dem Drive-Ordner das Archiv für das passende System herunterladen, entpacken und die Anwendung starten. Unity wird nicht benötigt.

## Steuerung

| Taste | Funktion |
| :---: | :---: |
| **← / →** | Schläger bewegen |
| **Leertaste** | Ball starten |
| **Esc** | Pause öffnen / schließen |

## Beschreibung des Projektes

Bauklotz ist ein 3D-Breakout (Arkanoid-Klon) im Bauhaus-Stil. Der Spieler lenkt einen Schläger und zerstört mit einem Ball alle Blöcke eines Levels. Jeder Blocktyp ist eine eigene Unity-Grundform: Würfel (1 Treffer), liegender Zylinder mit drei Ringen (3 Treffer, pro Treffer verschwindet ein Ring) und Halbkugel, die ein zufälliges Power-Up fallen lässt.

### Features

- Drei Level mit steigender Schwierigkeit, automatisch generiert aus Level-Daten (ScriptableObject)
- Leben, Punkte und Spielzustände über einen zentralen `GameManager` mit Events
- HUD mit Punkten (deutsches Format), Leben, Level-Fortschritt und Pause
- Pause-Menü, „Level geschafft“- und „Game Over“-Screen
- Vier Power-Ups: Breiter Schläger, Multi-Ball, Zeitlupe (mit längeren Schatten), Extraleben

## Verwendete Technologien

- Unity 6 mit Universal Render Pipeline (URP)
- Neues Input System
- C#: Events, Vererbung (Basisklasse `PowerUp`), ScriptableObjects, Prefabs und Prefab-Varianten
- Physik: Rigidbody, eigenes Abprallen per `Vector3.Reflect`, konstante Ballgeschwindigkeit
- TextMeshPro und uGUI

## Besondere Herausforderungen / Lessons Learned

**Vererbung**
- Alle Power-Ups erben von der abstrakten Basisklasse `PowerUp`, die Start, Ende und Timer übernimmt. Die Unterklassen (`WidePaddlePowerUp`, `MultiBallPowerUp`, `SlowMotionPowerUp`, `ExtraLifePowerUp`) implementieren nur `OnBegin` und `OnEnd`. Eine neue Power-Up-Art lässt sich so ohne Umbau ergänzen.

**Prefabs gezielt einsetzen**
- Prefab-Varianten statt Kopien: Block_Senf und Block_Salbei sind Varianten von Block_Rot und überschreiben nur das Material, ebenso die Power-Up-Münzen. Änderungen am Original gelten automatisch für alle Varianten.
- Zusammengesetzte Objekte (Schläger aus Körper und Kappen, stabiler Block mit drei Ringen) sind ein Prefab mit einem gemeinsamen Collider am Eltern-Objekt.
- Prefabs können keine Szenenobjekte referenzieren, deshalb suchen Blöcke den `GameManager` selbst. Der `LevelGenerator` erzeugt das ganze Raster per Script aus diesen Prefabs.

**Canvas und UI-Darstellung**
- Canvas mit Scale With Screen Size (1920×1080) und Anchor-Presets, damit das HUD bei jeder Auflösung an seinem Platz bleibt.
- Wiederverwendbare UI-Prefabs (Panel, Chip, Button in drei Varianten) im gleichen Stil: abgerundete Fläche, Kontur und harter Schlagschatten, nur mit Unity-Bordmitteln (Image, Outline, Shadow).
- Fortschrittsbalken über `Image Type: Filled`, Zahlen deutsch formatiert, Overlays mit abdunkelndem Hintergrund.

**Physik und Spiellogik**
- Abprallen ohne Physics Material selbst berechnet, inklusive Schutz gegen flache und senkrechte Dauerschleifen.
- Multi-Ball: Leben wird erst abgezogen, wenn der letzte Ball verloren ist.
- Kommunikation zwischen Spiellogik und HUD über Events, damit sich die Scripts nicht gegenseitig kennen müssen.

## Lizenz

Der Code und die Szenen dieses Projekts sind eine Studienarbeit. Die Schriften Unbounded und Archivo stehen unter der SIL Open Font License 1.1 (OFL).
