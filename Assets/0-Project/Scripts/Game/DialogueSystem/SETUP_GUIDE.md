# Diyalog Sistemi Kurulum Rehberi

Bu rehber, dedektif hikaye oyunu için oluşturulan diyalog sisteminin Unity'de nasıl kurulacağını açıklar.

## Dosya Yapısı

```
Assets/0-Project/Scripts/Game/DialogueSystem/
├── DialogueEnums.cs          - Karakter tipleri ve aksiyonlar
├── DialogueData.cs           - Veri yapıları (DialogueLine, DialogueChoice, DialogueNode)
├── DialogueDatabase.cs       - ScriptableObject veritabanı
├── DialogueManager.cs        - Ana diyalog kontrolcüsü
├── StoryStateManager.cs      - Flag sistemi ve hikaye durumu
├── CharacterDialoguePanel.cs - Her karakter UI paneli için
├── DialogueActionHandler.cs  - Minigame ve zorunlu geçişler
└── DialogueDataInitializer.cs - Örnek diyalog verileri (opsiyonel)
```

---

## Adım 1: Manager GameObject'lerini Oluştur

1. Sahnede boş bir GameObject oluştur: `DialogueSystem`
2. Şu scriptleri ekle:
   - `StoryStateManager`
   - `DialogueManager`
   - `DialogueActionHandler`

---

## Adım 2: DialogueDatabase Oluştur

1. Project panelinde: **Right Click > Create > Dialogue > Database**
2. Yeni asset'i `MainDialogueDatabase` olarak adlandır
3. `DialogueManager`'ın `DialogueDatabase` alanına bu asset'i sürükle

---

## Adım 3: Karakter Panellerini Ayarla

Her karakterin diyalog UI paneline `CharacterDialoguePanel` scripti ekle:

```
Inspector Ayarları:
├── Character Type: [Karakter seç]
├── Character Display Name: "Aşçı Fadime"
├── Dialogue Panel: [Panel GameObject]
├── Speaker Name Text: [TMP_Text]
├── Dialogue Text: [TMP_Text]
├── Continue Button: [Button]
├── Choices Panel: [Panel GameObject]
├── Choice Button A: [Button]
├── Choice Button B: [Button]
├── Choice Text A: [TMP_Text]
└── Choice Text B: [TMP_Text]
```

---

## Adım 4: TouchableObjects Ayarla

Her tıklanabilir karaktere:

1. `TouchableObjects` scriptinde yeni alanları doldur:
   - **Character Type**: Hangi karakter?
   - **Dialogue Panel**: Bu karakterin `CharacterDialoguePanel` scripti

---

## Adım 5: DialogueActionHandler Referansları

`DialogueActionHandler` Inspector'ında:

1. Her karakterin panelini ilgili alana sürükle
2. Minigame canvas'larını ata
3. Final decision panelini ata (varsa)

---

## Adım 6: Diyalog Verilerini Gir

### Seçenek A: Inspector'dan Elle Giriş

1. `DialogueDatabase` asset'ini aç
2. Characters listesine yeni karakter ekle
3. Her karakter için `dialogueNodes` listesine node'lar ekle

### Seçenek B: Script ile Otomatik

1. `DialogueDataInitializer` scriptini bir GameObject'e ekle
2. `Target Database` alanına database'i ata
3. Context Menu'den "Initialize Chapter 1 Data" çalıştır

---

## Örnek Diyalog Node Yapısı

```
Node ID: "ch1_entry"
Editor Note: "Aşçı ile ilk konuşma"

Lines:
  [0] Speaker: Dedektif
      Text: "Aşçı hanım iyi günler..."
  [1] Speaker: AsciFadime
      Text: "Anladık evladım..."

Choices:
  [0] Choice Text: "Kabul et"
      Action On Select: StartMinigame_FruitNinja
      Flag To Set: "accepted_help"

Next Node ID: (boş - seçenek var)
On Complete Action: None
Minimum Chapter: 1
```

---

## Flag Sistemi

### Otomatik Flagler:
- `talked_{CharacterType}` - Karakterle konuşulduğunda
- `chapter_{N}_started` - Chapter başladığında

### Manuel Flagler (Seçeneklerde):
- `fruitninja_completed`
- `tableclean_completed`
- `tuccar_hint_besteci`
- `simyaci_accused_beatrice`
- vb.

### Unavailable Flagler:
- `besteci_unavailable` - Karakter konuşulamaz
- `simyaci_unavailable` - Karakter konuşulamaz

---

## Minigame Entegrasyonu

1. Minigame bittiğinde `DialogueActionHandler.Instance.OnMinigameComplete()` çağır
2. Handler sonraki diyaloğu otomatik başlatır

---

## Test

1. Play mode'a gir
2. Bir karaktere tıkla
3. Inspector karaktere yürür
4. Diyalog paneli otomatik açılır
5. Continue butonu ile ilerle
6. Seçenekler geldiğinde tıkla

---

## Sorun Giderme

| Sorun | Çözüm |
|-------|-------|
| Diyalog açılmıyor | TouchableObjects'te DialoguePanel atandı mı? |
| Seçenekler görünmüyor | ChoicesPanel ve butonlar atandı mı? |
| Karakter tıklanamıyor | `isAvailable` true mu? Unavailable flag var mı? |
| Minigame sonrası devam etmiyor | OnMinigameComplete çağrılıyor mu? |
