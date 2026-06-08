# Felony-Drive
Hra je o simulaci řízení auta na styl [GTA 1](https://cs.wikipedia.org/wiki/Grand_Theft_Auto_(videohra)).
## :computer: Nástroje a technologie 
- **Herní engine:** Unity (s využitím Universal Render Pipeline pro moderní 2D nasvícení)
- **Programování:** C# (využití nového Unity Input Systemu pro plynulé ovládání)
- **Grafika:** Libreprite/Aseprite (vlastní pixel-artové kresby a animace)
- **Verzování:** Git
## :world_map: Roadmap 
- [x] :car: Semi-realistická fyzika vozidla 
- [x] :movie_camera: Dynamická kamera reagující na rychlost 
- [x] :sound: :tv: Audio-vizuální efekty 
- [x] :joystick: Uživatelské rozhraní (tachometr, RPM, palivo, nitro, rádio)
- [x] :city_sunrise: Prostředí 
- [x] :goal_net: Mise (doručovací mise s minimapou a navigační šipkou)
- [x] :gear: Manuální / automatická převodovka s RPM křivkou
- [x] :rocket: Nitro boost systém s vlastní zásobou
- [x] :fuelpump: Systém paliva a čerpací stanice
- [x] :radio: Rádio se stanicemi (Mirror – zrcadlení systémového přehrávače, Stanice 1 – in-game hudba)
- [x] :sunny: :crescent_moon: Denní a noční cyklus s dynamickým osvětlením
- [x] :floppy_disk: Ukládání hry (poloha, směr, peníze, palivo, nitro)
- [x] :moneybag: Systém peněženky hráče a odměn za splněné mise
- [x] :arrows_counterclockwise: Přebindování kláves v nastavení
## :keyboard: Ovládání

### Jízda
| Klávesa | Akce |
|---|---|
| `W` | Plyn |
| `S` | Brzda / Couvání |
| `A` | Zatáčení vlevo |
| `D` | Zatáčení vpravo |
| `Mezerník` | Ruční brzda |

### Převodovka
| Klávesa | Akce |
|---|---|
| `↑` | Zařadit vyšší rychlostní stupeň |
| `↓` | Zařadit nižší rychlostní stupeň |
| `B` | Přepnout automatické / manuální řazení |

### Vozidlo
| Klávesa | Akce |
|---|---|
| `E` | Nastartovat / Zastavit motor |
| `H` | Troubení (drž pro opakované troubení) |
| `L` | Světla zapnout / vypnout |
| `Shift` | Nitro (přepínač) |
| `I` | Natankovat *(pouze u čerpací stanice)* |

### Menu
| Klávesa | Akce |
|---|---|
| `P` | Pauza |
