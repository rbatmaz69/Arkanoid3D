# Bauklotz – Breakout aus Grundformen

Refik Bilal Batmaz 215807

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
- Eigene Kollisionsberechnung ohne Unity-Physik (kein Rigidbody, keine Collider): Kreis gegen Rechteck bzw. Kapsel auf der X/Z-Ebene, Abprallen per `Vector3.Reflect`, konstante Ballgeschwindigkeit
- TextMeshPro und uGUI

## Besondere Herausforderungen / Lessons Learned

**Vererbung**
- Alle Power-Ups erben von der abstrakten Basisklasse `PowerUp`, die Start, Ende und Timer übernimmt. Die Unterklassen (`WidePaddlePowerUp`, `MultiBallPowerUp`, `SlowMotionPowerUp`, `ExtraLifePowerUp`) implementieren nur `OnBegin` und `OnEnd`. Eine neue Power-Up-Art lässt sich so ohne Umbau ergänzen.

**Prefabs gezielt einsetzen**
- Prefab-Varianten statt Kopien: Block_Senf und Block_Salbei sind Varianten von Block_Rot und überschreiben nur das Material, ebenso die Power-Up-Münzen. Änderungen am Original gelten automatisch für alle Varianten.
- Zusammengesetzte Objekte (Schläger aus Körper und Kappen, stabiler Block mit drei Ringen) sind ein Prefab mit einer gemeinsamen Kollisionsform (`BoxBody`) am Eltern-Objekt.
- Prefabs können keine Szenenobjekte referenzieren, deshalb suchen Blöcke den `GameManager` selbst. Der `LevelGenerator` erzeugt das ganze Raster per Script aus diesen Prefabs.

**Power-Up-Auswahl**
- Es gibt keine feste Reihenfolge. Jeder Halbkugel-Block wählt beim Zerstören zufällig eines seiner vier Power-Ups (gleich wahrscheinlich) und lässt es fallen. Der Spieler sammelt es mit dem Schläger ein.
- Erneutes Einsammeln von Breit oder Zeitlupe ersetzt den laufenden Effekt, statt ihn zu stapeln. Die Laufzeit startet neu.

**Canvas und UI-Darstellung**
- Canvas mit Scale With Screen Size (1920×1080) und Anchor-Presets, damit das HUD bei jeder Auflösung an seinem Platz bleibt.
- Wiederverwendbare UI-Prefabs (Panel, Chip, Button in drei Varianten) im gleichen Stil: abgerundete Fläche, Kontur und harter Schlagschatten, nur mit Unity-Bordmitteln (Image, Outline, Shadow).
- Fortschrittsbalken über `Image Type: Filled`, Zahlen deutsch formatiert, Overlays mit abdunkelndem Hintergrund.

**Kollisionen ohne Physik-Engine**
- Das Spiel läuft effektiv in der X/Z-Ebene, deshalb reicht 2D-Geometrie: Der Ball ist ein Kreis, Wände, Blöcke, Bodensensor und Power-Ups sind achsparallele Rechtecke (`BoxBody`), der Schläger ist eine Kapsel (Strecke plus Radius, `PaddleController`). Die Mathematik steckt in `CollisionWorld`.
- Der `BallController` bewegt sich selbst und teilt jeden Frame in kleine Schritte (höchstens ein halber Ballradius), damit er auch bei langen Frames nicht durch Wände tunnelt. Bei einer Überlappung wird er entlang der Normale herausgeschoben und reflektiert, aber nur, wenn er in die Fläche hineinfliegt. So gibt es an Ecken keine doppelte Reflexion.
- Blöcke reagieren über das Interface `IBallHitReceiver`, der Bodensensor wird vom Ball nach jedem Schritt geprüft, Power-Ups prüfen selbst, ob sie den Schläger berühren. Pause funktioniert weiter über `Time.timeScale`, weil alles mit `Time.deltaTime` rechnet.
- Abprallen selbst berechnet, inklusive Schutz gegen flache und senkrechte Dauerschleifen.
- Multi-Ball: Leben wird erst abgezogen, wenn der letzte Ball verloren ist.
- Kommunikation zwischen Spiellogik und HUD über Events, damit sich die Scripts nicht gegenseitig kennen müssen.

## Lizenz

Der Code und die Szenen dieses Projekts sind eine Studienarbeit. Die Schriften Unbounded und Archivo stehen unter der SIL Open Font License 1.1 (OFL).
