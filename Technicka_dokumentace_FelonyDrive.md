# Technická dokumentace – Felony Drive

Felony Drive je 2D závodní hra vytvořená v enginu Unity. Hráč řídí auto po herním světě, plní doručovací mise, tankuje na benzínové stanici a vydělává peníze. Hra obsahuje fyzikální model auta s převodovkou, dynamickým gripy, nitrem a palivem. Veškerý postup se ukládá do souboru JSON.

Hra umožňuje:
- Ovládání auta s manuální nebo automatickou převodovkou
- Plnění doručovacích misí a sbírání odměn
- Tankování paliva a nitra na benzínové stanici
- Poslouchání rádia (systémové audio nebo interní stanice)
- Nastavení grafiky, zvuku a klávesových zkratek
- Ukládání a načítání herního stavu

---

## Struktura projektu

Projekt je tvořen samostatnými třídami – MonoBehaviours – přiřazenými k objektům ve scéně. Třídy jsou rozděleny do tematických skupin.

| Skupina | Scripty |
|---|---|
| Řízení auta | CarControllerV2, CarGearBox, CarEffects, CarFuel, CarNitro, CarRadio |
| Uživatelské rozhraní | UI, UIData, MinimapArrow |
| Herní systémy | CameraController, DayNightCycle, PlayerWallet, SaveData |
| Mise a interakce | DeliveryMission, DeliveryDropOff, GasStation, TriggerChecker |
| Menu a nastavení | MainMenu, Settings, Pause |
| Editor nástroje | FullSreen (pouze v Unity Editoru) |
| Vstup (autogenerováno) | PlayerActions |

---

## Přehled klávesových zkratek

| Klávesa | Akce |
|---|---|
| W / S | Plyn / Brzda nebo zpátečka |
| A / D | Zatáčení vlevo / vpravo |
| Mezerník | Ruční brzda |
| E | Start / stop motoru |
| L | Světla zapnout / vypnout |
| H | Houkání |
| Šipka nahoru / dolů | Řazení nahoru / dolů |
| Shift | Nitro |
| B | Přepnutí automat / manuál |
| P | Pauza |
| I (u benzínky) | Tankování |

---

## Přehled všech tříd

| Třída | Soubor | Účel |
|---|---|---|
| CarControllerV2 | CarControllerV2.cs | Hlavní fyzikální řadič auta |
| CarGearBox | CarGearBox.cs | Převodovka a výpočet RPM |
| CarEffects | CarEffects.cs | Zvukové a vizuální efekty auta |
| CarFuel | CarFuel.cs | Správa paliva |
| CarNitro | CarNitro.cs | Správa a logika nitra |
| CarRadio | CarRadio.cs | Rádio s podporou systémového přehrávače |
| CameraController | CameraController.cs | Sledování hráče kamerou |
| DayNightCycle | DayNightCycle.cs | Cyklus dne a noci |
| UIData | UIData.cs | Sdílená data pro UI (ScriptableObject) |
| UI | UI.cs | HUD – rychloměr, tachometr, palivo, nitro |
| MinimapArrow | MinimapArrow.cs | Šipka na minimapě ukazující k cíli |
| PlayerWallet | PlayerWallet.cs | Správa peněz hráče |
| SaveData | SaveData.cs | Ukládání a načítání hry do JSON |
| TriggerChecker | TriggerChecker.cs | Obecný detektor vstupu/výstupu z triggeru |
| GasStation | GasStation.cs | Benzínová stanice – tankování paliva a nitra |
| DeliveryMission | DeliveryMission.cs | Spuštění doručovací mise |
| DeliveryDropOff | DeliveryDropOff.cs | Dokončení mise a výplata odměny |
| MainMenu | MainMenu.cs | Hlavní menu hry |
| Settings | Settings.cs | Nastavení grafiky, zvuku a kláves |
| Pause | Pause.cs | Pozastavení hry |
| FullscreenHotkeyHandler | FullSreen.cs | Editor nástroj pro fullscreen zobrazení |

---

## Pomocné třídy (definovány uvnitř skriptů)

| Třída | Definována v | Účel |
|---|---|---|
| Gear | CarGearBox.cs | Serializovatelná data jednoho stupně převodovky |
| Gauge | UI.cs | Ovládání ručičkového budíku v HUD |
| Gauge.GaugeNeedle | UI.cs | Otáčení ručičky budíku |
| ButtonHandler | UI.cs | Obsluha tlačítek v herním HUD |
| MenuButton | MainMenu.cs, Settings.cs | Obsluha tlačítek hlavního menu |
| SaveData.SaveDataObj | SaveData.cs | Serializovatelný objekt uložené hry |

---

## Dokumentace tříd

---

### CarControllerV2

```csharp
public class CarControllerV2 : MonoBehaviour
```

Hlavní skript pro ovládání auta. Zpracovává vstup hráče, počítá fyziku pohybu, řízení a simuluje grip pneumatik pomocí sil aplikovaných na přední a zadní nápravu.

#### Veřejné proměnné

| Proměnná | Typ | Popis |
|---|---|---|
| maxSpeed | float | Maximální rychlost auta dopředu (m/s) |
| maxReverseSpeed | float | Maximální rychlost jízdy vzad |
| weight | float | Hmotnost auta (předána Rigidbody2D) |
| steeringPower | float | Síla točivého momentu při zatáčení |
| brakeForce | float | Odpor při brzdění |
| cruiseDamping | float | Odpor při jízdě bez plynu |
| frontGrip | float | Boční grip přední nápravy |
| rearGrip | float | Boční grip zadní nápravy |
| axleDistance | float | Vzdálenost nápravy od středu auta |
| optimalSteeringSpeed | float | Rychlost (m/s) s nejúčinnějším zatáčením |
| autoShifting | bool | Přepínač automatické převodovky |
| engineStarted | bool | Stav motoru (zapnutý / vypnutý) |
| isHandbrake | bool | Stav ruční brzdy |
| isBraking | bool | Příznak aktivního brzdění |
| isHonking | bool | Příznak aktivního houkání |

#### Vlastnosti (Properties)

| Vlastnost | Typ | Popis |
|---|---|---|
| speed | float | Okamžitá rychlost (velikost vektoru rychlosti) |
| throttleInput | float | Vstup plynu / brzdy (-1 až 1) |
| steeringInput | float | Vstup řízení (-1 až 1) |
| normalizedSpeed | float | Rychlost normalizovaná na rozsah 0–1 |
| CarCoords | Vector2 | Aktuální pozice auta ve světě |
| Heading | float | Aktuální natočení auta (euler Z) |

#### Metody

| Metoda | Popis |
|---|---|
| SetHeading(float heading) | Nastaví natočení auta na zadaný úhel |
| SetCoords(Vector2 coords) | Nastaví pozici auta |
| EngineStart() | Přepíná stav motoru a spouští zvuk |
| GetInputs() | Čte vstup z PlayerActions každý frame |
| UpdateSpeed() | Aplikuje hnací síly, brzdné síly a grip pneumatik |
| AutoShift() | Logika automatického řazení |
| GetSteeringMultiplier() | Vrátí koeficient účinnosti řízení podle rychlosti |

#### Průběh FixedUpdate

1. Čte vstup hráče přes `GetInputs()`
2. Volá `UpdateSpeed()`:
   - Pokud je motor zapnutý a hráč dává plyn → přidá sílu ve směru auta
   - Pokud hráč brzdí → zvýší lineárníDamping
   - Vypočítá boční rychlosti přední a zadní nápravy
   - Aplikuje třecí síly pro simulaci gripu pneumatik

---

### CarGearBox

```csharp
public class CarGearBox : MonoBehaviour
```

Spravuje převodovku auta. Uchovává pole rychlostních stupňů, řeší řazení nahoru/dolů a vypočítává aktuální RPM motoru.

#### Třída Gear (serializovatelná)

```csharp
[System.Serializable]
public class Gear
```

| Pole | Typ | Popis |
|---|---|---|
| name | string | Název stupně (např. "R", "N", "1", "2"…) |
| gearAcceleration | float | Zrychlující síla tohoto stupně |
| maxSpeed | float | Maximální rychlost v tomto stupni (m/s) |

#### Veřejné proměnné CarGearBox

| Proměnná | Typ | Popis |
|---|---|---|
| gears | Gear[] | Pole všech rychlostních stupňů |
| maxRPM | float | Maximální otáčky motoru |
| currentGear | int | Index aktuálního stupně v poli |
| rpmCurve | AnimationCurve | Křivka závislosti RPM na rychlosti |
| rpm | float | Aktuální otáčky motoru |
| CurrentGear | Gear | Aktuální stupeň (property) |

#### Metody

| Metoda | Popis |
|---|---|
| ShiftUp() | Zařadí o stupeň výš (pokud není na maximu) |
| ShiftDown() | Zařadí o stupeň níž (pokud není na minimu) |

#### Výpočet RPM

Pokud je auto v neutrálu (index 1), RPM se ovlivňuje přímo plynem hráče v rozsahu 1 000–maxRPM. V ostatních stupních se RPM počítá jako:

```
rpm = 1000 + rpmCurve.Evaluate(|speed / maxSpeedStupně|) * 4000
```

---

### CarEffects

```csharp
public class CarEffects : MonoBehaviour
```

Spravuje všechny vizuální a zvukové efekty auta. Reaguje na stav auta (drift, brzdění, RPM, motor).

#### Veřejné proměnné

| Proměnná | Typ | Popis |
|---|---|---|
| engineStartAudioClip | AudioClip | Zvuk nastartování motoru |
| engineLoopAudioClip | AudioClip | Zvuk běžícího motoru (smyčka) |
| honkAudioClip | AudioClip | Zvuk houkání |
| driftThreshold | float | Boční rychlost nápravy spouštějící efekt driftu |
| trailRenderers | TrailRenderer[] | Stopy pneumatik (2 kusy) |
| driftParticles | ParticleSystem[] | Částicové efekty driftu (2 kusy) |
| headlight | Light2D[] | Světla auta |

#### Metody

| Metoda | Popis |
|---|---|
| Honk() | Přehraje nebo zastaví zvuk houkání |
| Lights(bool state) | Zapne/vypne světla a změní barvu HUD |
| StartEngineSound() | Spustí nebo zastaví zvuk motoru |
| UpdateVisuals() | Aktivuje/deaktivuje stopy a částice při driftu |
| UpdateAudio() | Nastaví pitch motorového zvuku podle RPM |

#### Průběh StartEngineSound

1. Pokud je motor zapnutý → spustí koroutin `EngineStartSequence()`
2. Přehraje zvuk nastartování (jednorázový)
3. Po jeho skončení přepne na smyčkový zvuk běžícího motoru
4. Animuje mírné "trhnutí" auta (změna scale s interpolací)

---

### CarFuel

```csharp
public class CarFuel : MonoBehaviour
```

Spravuje zásobu paliva auta. Spotřeba je závislá na zatížení motoru (RPM).

#### Proměnné

| Proměnná | Typ | Popis |
|---|---|---|
| maxFuel | float | Maximální kapacita nádrže (výchozí 75) |
| currentFuel | float | Aktuální množství paliva (property, private set) |
| fuelConsumptionRate | float | Základní spotřeba za sekundu |

#### Metody

| Metoda | Popis |
|---|---|
| SetFuel(float amount) | Nastaví palivo na zadanou hodnotu |
| AddFuel(float amount) | Přidá palivo (nepřekročí maximum) |

#### Spotřeba paliva

V každém `Update()` (pokud hráč není na benzínce):

```
engineLoad = rpm / maxRPM
fuelBurned = fuelConsumptionRate * engineLoad * Time.deltaTime
```

---

### CarNitro

```csharp
public class CarNitro : MonoBehaviour
```

Spravuje zásobu nitra a jeho efekt na rychlost.

#### Proměnné

| Proměnná | Typ | Popis |
|---|---|---|
| maxNitro | float | Maximální kapacita nitra |
| nitroBoost | float | Multiplikátor síly při aktivním nitru |
| nitroConsumptionRate | float | Spotřeba nitra za sekundu |
| currentNitro | float | Aktuální množství nitra (property) |
| nitroActive | bool | Příznak aktivního nitra (property) |

#### Metody

| Metoda | Popis |
|---|---|
| ToggleNitro() | Zapne/vypne nitro (jen pokud není prázdné) |
| SetNitro(float amount) | Nastaví nitro na zadanou hodnotu |
| AddNitro(float amount) | Přidá nitro (v rozsahu 0–max) |

#### Chování

Nitro se spotřebovává pouze pokud je aktivní **a** auto se pohybuje (`normalizedSpeed > 0.1`). Když dojde nitro, automaticky se deaktivuje. Multiplikátor `nitroBoost` je aplikován v `CarControllerV2` při výpočtu hnací síly.

---

### CarRadio

```csharp
public class CarRadio : MonoBehaviour
```

Ovládá herní rádio. Podporuje dvě stanice: **Mirror** (přečte přehrávané médium ze systému Windows přes nativní plugin) a **InGame1** (interní AudioClip).

#### Stanice (RadioStation enum)

| Hodnota | Popis |
|---|---|
| Mirror | Zrcadlí aktuálně přehrávanou hudbu v systému Windows |
| InGame1 | Interní herní stanice s AudioClipem |

#### Veřejné metody

| Metoda | Popis |
|---|---|
| NextStation(int direction) | Přepne stanici ve směru +1 nebo -1 |
| NextSong(int direction) | Přeskočí skladbu dopředu/dozadu (jen Mirror) |
| PlayPause() | Přehraje/pozastaví (jen Mirror) |

#### Privátní metody

| Metoda | Popis |
|---|---|
| ChangeStation(RadioStation) | Přepne na danou stanici a inicializuje její chování |
| StartMirrorLoop() | Spustí asynchronní smyčku dotazující Windows Media Session |
| StopMirrorLoop() | Bezpečně ukončí smyčku přes CancellationToken |
| SpotifyPollingLoop(token) | Asynchronní smyčka, každé 2 s načte název a interpreta |

#### Poznámka k nativnímu pluginu

Plugin `MediaPlugin.dll` komunikuje se systémem Windows a poskytuje funkce `Init`, `Shutdown`, `Refresh`, `GetTitle`, `GetArtist`, `TogglePlayPause`, `Next`, `Prev`. Volání probíhají asynchronně (`Task.Run`), aby neblokovala herní vlákno Unity.

---

### CameraController

```csharp
public class CameraController : MonoBehaviour
```

Sleduje hráče kamerou. Zoom kamery a posun dopředu jsou závislé na aktuální rychlosti auta.

#### Proměnné

| Proměnná | Typ | Popis |
|---|---|---|
| target | Transform | Transform sledovaného objektu (auto) |
| offset | Vector3 | Základní offset kamery od hráče |
| cameraMoveSpeed | float | Rychlost pohybu kamery (MoveTowards) |
| maxZoomOut | float | Maximální orthographicSize při max. rychlosti |
| maxZoomIn | float | Minimální orthographicSize při stání |
| zoomSmoothness | float | Plynulost interpolace zoomu |
| maxForwardOffset | float | Maximální posun kamery dopředu při max. rychlosti |

#### Chování (LateUpdate)

1. Vypočítá `speedPerc` = aktuální rychlost / maximální rychlost
2. Interpoluje cílový zoom mezi `maxZoomIn` a `maxZoomOut`
3. Interpoluje dopředný posun kamery (0 až `maxForwardOffset`)
4. Pohybuje kamerou pomocí `Vector3.MoveTowards`
5. Plynule mění `orthographicSize` pomocí `Lerp`

---

### DayNightCycle

```csharp
public class DayNightCycle : MonoBehaviour
```

Simuluje cyklus dne a noci pomocí globálního 2D světla.

#### Proměnné

| Proměnná | Typ | Popis |
|---|---|---|
| globalLight | Light2D | Globální světlo scény |
| dayCycle | Gradient | Gradient barev průběhu dne |
| dayLength | float | Délka jednoho dne v sekundách |
| timeNow | float | Aktuální čas cyklu (0 = úsvit, 1 = konec dne) |

#### Chování

Každý frame se `timeNow` zvyšuje o `Time.deltaTime / dayLength`. Po dosažení hodnoty 1 se resetuje na 0. Barva světla je vyhodnocena z gradientu a intenzita se mění podle sinusoidy přes `timeNow * PI * 2`.

---

### UIData

```csharp
[CreateAssetMenu]
public class UIData : ScriptableObject
```

Sdílený ScriptableObject předávající herní data do UI bez přímé vazby na konkrétní scénu.

#### Vlastnosti

| Vlastnost | Typ | Popis |
|---|---|---|
| gear | string | Název aktuálního rychlostního stupně |
| radioTrack | string | Název přehrávané skladby |
| radioChannel | string | Název aktuální rozhlasové stanice |
| cash | float | Aktuální stav konta hráče |

---

### UI

```csharp
public class UI : MonoBehaviour
```

Hlavní skript herního HUD. Ovládá budíky rychlosti, otáček a paliva, animuje indikátor nitra a registruje tlačítka rádia.

#### Pomocné třídy

**Gauge** – reprezentuje jeden budík (ručičkový ukazatel).

| Metoda | Popis |
|---|---|
| Initialize(name, doc) | Najde VisualElement budíku v UIDocument |
| SetTint(Color) | Změní barevný nádech grafiky budíku |

**Gauge.GaugeNeedle** – ručička budíku.

| Metoda | Popis |
|---|---|
| Initialize(name, doc) | Najde VisualElement ručičky |
| UpdateNeedle(float normalizedValue) | Nastaví otočení ručičky lineárním lerp mezi MinAngle a MaxAngle |

**ButtonHandler** – napojí funkci na kliknutí tlačítka v UIDocument.

#### Metody UI

| Metoda | Popis |
|---|---|
| UpdateNitroUI() | Animuje hladinu nitra (pohyb kapaliny při akceleraci a zatáčení) |
| ChangeSpeedometerTint(bool) | Mění barvu budíků podle stavu světel auta |

#### Průběh Update

Každý frame normalizuje hodnoty rychlosti, RPM a paliva a volá `UpdateNeedle()` na každý budík. Dále aktualizuje animaci nitra.

---

### MinimapArrow

```csharp
public class MinimapArrow : MonoBehaviour
```

Zobrazuje šipku na minimapě ukazující směrem k aktivnímu cíli mise.

#### Proměnné

| Proměnná | Typ | Popis |
|---|---|---|
| target (static) | Transform | Cíl mise (pickup nebo dropoff) |
| player | Transform | Transform hráče |

#### Chování

Pokud je `target == null`, šipka se skryje. Jinak vypočítá úhel mezi hráčem a cílem pomocí `Atan2` a nastaví rotaci objektu.

---

### PlayerWallet

```csharp
public class PlayerWallet : MonoBehaviour
```

Statická správa konta hráče. Hodnota `cash` je dostupná odkudkoli ve hře.

#### Proměnné a metody

| Člen | Typ | Popis |
|---|---|---|
| cash | float (static) | Aktuální hotovost hráče (výchozí 250) |
| Add(float amount) | static void | Přidá částku a aktualizuje UIData |
| Subtract(float amount) | static void | Odečte částku a aktualizuje UIData |
| SetCash(float amount) | static void | Nastaví hotovost na přesnou hodnotu |

---

### SaveData

```csharp
public class SaveData : MonoBehaviour
```

Ukládá a načítá herní stav do/ze souboru `save.json` v `Application.persistentDataPath`.

#### Třída SaveDataObj (serializovatelná)

| Pole | Typ | Popis |
|---|---|---|
| cash | float | Hotovost hráče |
| fuel | float | Množství paliva |
| nitro | float | Množství nitra |
| coords | Vector2 | Pozice auta |
| heading | float | Natočení auta |

#### Metody

| Metoda | Popis |
|---|---|
| SaveGameplayData() | Serializuje aktuální stav do JSON a zapíše soubor |
| ReadGameplayData() | Načte JSON ze souboru a aplikuje data do hry |
| SaveDataObj.GetData() | Sestaví objekt s aktuálními hodnotami |
| SaveDataObj.LoadData() | Aplikuje uložená data zpět do hry |

#### Cesta k souboru

```
Application.persistentDataPath + "/save.json"
```

Na Windows typicky: `%APPDATA%\..\LocalLow\<Company>\<Product>\save.json`

---

### TriggerChecker

```csharp
public class TriggerChecker : MonoBehaviour
```

Obecný detektor vstupu a výstupu hráče z 2D triggeru. Umožňuje přiřadit libovolné akce bez podtřídění.

#### Události

| Událost | Typ | Popis |
|---|---|---|
| onTriggered | System.Action | Vyvolána při vstupu hráče do triggeru |
| onExit | System.Action | Vyvolána při výstupu hráče z triggeru |

Reaguje pouze na objekty s tagem `"Player"`.

---

### GasStation

```csharp
public class GasStation : MonoBehaviour
```

Benzínová stanice doplňující palivo a nitro za peníze.

#### Proměnné

| Proměnná | Typ | Popis |
|---|---|---|
| flowRate | float | Rychlost doplňování paliva (jednotky/s) |
| playerInRange (static) | bool | Hráč je v dosahu stanice |
| triggers | Collider2D[] | Triggery stanice (přiřazovány dynamicky) |
| text | TextMeshPro | Text zobrazující instrukce a stav paliva |

#### Chování

Při startu přiřadí `TriggerChecker` ke každému triggeru. Pokud hráč stojí v triggeru a drží klávesu **I** a má dostatek peněz, doplňuje se palivo rychlostí `flowRate` za cenu `flowRate * 3` za sekundu. Nitro se doplňuje rychlostí `flowRate * 0.5` za sekundu.

---

### DeliveryMission

```csharp
public class DeliveryMission : MonoBehaviour
```

Spouštěč doručovací mise. Aktivuje se při vstupu hráče do pickup zóny.

#### Proměnné

| Proměnná | Typ | Popis |
|---|---|---|
| dropoff | GameObject | Objekt cílové dropoff zóny |
| isDelivering (static) | bool | Příznak probíhající mise |

#### Chování

Při vstupu hráče (`OnTriggerEnter2D`):
1. Zkontroluje, zda neprobíhá jiná mise
2. Nastaví `isDelivering = true`
3. Nastaví `MinimapArrow.target` na dropoff objekt
4. Aktivuje dropoff zónu
5. Zničí sám sebe

---

### DeliveryDropOff

```csharp
public class DeliveryDropOff : MonoBehaviour
```

Cílová zóna doručovací mise.

#### Proměnné

| Proměnná | Typ | Popis |
|---|---|---|
| reward | float | Finanční odměna za dokončení mise |

#### Chování

Při vstupu hráče:
1. Nastaví `isDelivering = false`
2. Přidá odměnu do `PlayerWallet`
3. Zničí sám sebe
4. Šipka na minimapě se automaticky skryje (cíl = null)

---

### MainMenu

```csharp
public class MainMenu : MonoBehaviour
```

Ovládá tlačítka hlavního menu.

#### Třída MenuButton (lokální)

Registruje kliknutí a hover efekt (přiblížení scale na 1.2) na tlačítku v UIDocument.

#### Metody MainMenu

| Metoda | Popis |
|---|---|
| Play() | Načte herní scénu `"Game"` |
| Quit() | Ukončí aplikaci |

Tlačítka `PlayButton`, `SettingsButton` a `QuitButton` jsou registrována v `OnEnable()`.

---

### Settings

```csharp
public class Settings : MonoBehaviour
```

Nastavení hry – grafika, zvuk a přemapování kláves. Nastavení jsou ukládána do `PlayerPrefs`.

#### Metody

| Metoda | Popis |
|---|---|
| SettingsToggle() | Zobrazí/skryje panel nastavení |
| SetupGraphics() | Inicializuje dropdowny pro rozlišení, mod obrazovky a kvalitu, toggle VSync |
| SetupAudio() | Inicializuje slidery Master/Music/SFX a aplikuje hodnoty na AudioMixer |
| SetupKeybinds() | Dynamicky generuje UI pro přemapování kláves pomocí Unity Input System rebinding API |

#### Ukládané hodnoty v PlayerPrefs

| Klíč | Typ | Popis |
|---|---|---|
| quality | int | Index úrovně kvality grafiky |
| vsync | int | VSync (0 nebo 1) |
| screenMode | int | Mód obrazovky (FullScreenMode) |
| resolution | string | Rozlišení ve formátu `"1920x1080"` |
| master | float | Hlasitost Master kanálu |
| music | float | Hlasitost Music kanálu |
| sfx | float | Hlasitost SFX kanálu |
| keybinds | string | JSON s přemapovanými klávesami |

---

### Pause

```csharp
public class Pause : MonoBehaviour
```

Zpracovává pauzu hry. Při stisku klávesy **P** zastaví čas a otevře panel nastavení.

#### Proměnné

| Proměnná | Typ | Popis |
|---|---|---|
| isPaused | bool | Aktuální stav pauzy |
| settings | Settings | Reference na skript Settings |

#### Chování

Při stisknutí pauzy:
- Přepíná `Time.timeScale` mezi 0 (pauza) a 1 (hra)
- Volá `settings.SettingsToggle()` pro zobrazení/skrytí menu

---

### FullscreenHotkeyHandler (editor only)

```csharp
#if UNITY_EDITOR
public class FullscreenHotkeyHandler : MonoBehaviour
```

Nástroj pro Unity Editor umožňující přepínání herního okna do fullscreen módu. **Není součástí sestavení hry.**

#### Chování

- Při startu automaticky přepne do fullscreen (pokud je `makeFullscreenAtStart = true`)
- Klávesa `\` (backslash) přepíná fullscreen za běhu v Editoru
- Dostupné také přes menu `Window → General → Game (Fullscreen)`

---

## Shrnutí

Felony Drive je rozčleněna do přehledných, samostatných tříd, kde každá třída nese jasně definovanou odpovědnost. Fyzikální model auta zajišťuje `CarControllerV2` ve spolupráci s `CarGearBox`, `CarFuel` a `CarNitro`. Veškeré efekty jsou odděleny do `CarEffects`. UI je postaveno na UI Toolkit a data jsou sdílena přes ScriptableObject `UIData`. Herní postup je perzistentní díky `SaveData` (JSON soubor) a nastavení hráče jsou uchována v `PlayerPrefs`.
