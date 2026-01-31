# Fruit Ninja Mini-Game - Setup Guide

Bu döküman Fruit Ninja mini-game'in Unity'de nasıl kurulacağını açıklar.

## Script'ler

Aşağıdaki script'ler oluşturuldu:

1. **FruitNinjaManager.cs** - Ana game manager
2. **SliceController.cs** - Mouse input ve kesme mekaniği
3. **SliceableObject.cs** - Meyve ve bomba base class
4. **FruitSpawner.cs** - Object spawning sistemi
5. **FruitNinjaUI.cs** - UI controller
6. **ObjectPool.cs** - Object pooling (opsiyonel)

## Unity'de Kurulum

### 1. Scene Hierarchy

```
FruitNinjaGame (GameObject)
├── GameManager (FruitNinjaManager component)
├── Spawner (FruitSpawner component)
├── SliceController (SliceController component)
└── Canvas (UI)
    ├── ScoreText (TextMeshPro)
    ├── TimerText (TextMeshPro)
    └── GameOverPanel (Panel)
        ├── Title (TextMeshPro)
        ├── Message (TextMeshPro)
        ├── RestartButton (Button)
        └── ExitButton (Button)
```

### 2. GameManager Setup

1. Yeni bir GameObject oluştur: **GameManager**
2. `FruitNinjaManager` component'ini ekle
3. Inspector'da ayarları yap:
   - **Game Duration:** 60 saniye
   - **Target Score:** 30 meyve
   - **Bomb Penalty:** 10 puan
   - **Fruit Spawner:** Spawner GameObject'i sürükle
   - **Slice Controller:** SliceController GameObject'i sürükle

### 3. Spawner Setup

1. Yeni bir GameObject oluştur: **Spawner**
2. `FruitSpawner` component'ini ekle
3. Inspector'da ayarları yap:
   - **Fruit Prefabs:** 4 meyve prefab'ını array'e ekle
   - **Bomb Prefab:** Bomba prefab'ını ekle
   - **Spawn Interval:** 1 saniye
   - **Bomb Spawn Chance:** 0.2 (20% bomba)
   - **Launch Force Min/Max:** 10-15
   - **Launch Angle Min/Max:** 60-120 derece
   - **Spawn Y Position:** -6 (ekranın altı)
   - **Spawn X Min/Max:** -7 ile 7 arası

### 4. SliceController Setup

1. Yeni bir GameObject oluştur: **SliceController**
2. `SliceController` component'ini ekle
3. Inspector'da ayarları yap:
   - **Min Slice Velocity:** 0.1
   - **Trail Width:** 0.1
   - **Trail Fade Speed:** 5
   - **Slice Radius:** 0.5
   - **Sliceable Layer:** "Sliceable" layer'ı oluştur ve seç

### 5. Fruit Prefab Oluşturma

Her meyve için:

1. Yeni bir GameObject oluştur
2. `SpriteRenderer` ekle ve meyve sprite'ını ata
3. `CircleCollider2D` veya `PolygonCollider2D` ekle
4. `Rigidbody2D` ekle:
   - **Body Type:** Dynamic
   - **Gravity Scale:** 1
5. `SliceableObject` script'ini ekle:
   - **Object Type:** Fruit
6. Layer'ı **"Sliceable"** olarak ayarla
7. Prefab olarak kaydet

### 6. Bomb Prefab Oluşturma

Bomba için aynı adımları tekrarla ama:
- **Object Type:** Bomb olarak ayarla
- Farklı bir sprite kullan (yumurta)

### 7. UI Setup

1. Canvas oluştur (sağ tık > UI > Canvas)
2. **ScoreText** oluştur (TextMeshPro):
   - Sol üst köşeye yerleştir
   - Text: "Score: 0"
   - Font size: 36
3. **TimerText** oluştur (TextMeshPro):
   - Sağ üst köşeye yerleştir
   - Text: "01:00"
   - Font size: 36
4. **GameOverPanel** oluştur (Panel):
   - Başlangıçta inactive
   - Ortalanmış, ekranın %60'ı kadar
   - İçine Title, Message, RestartButton, ExitButton ekle
5. `FruitNinjaUI` component'ini Canvas'a ekle
6. Referansları bağla

### 8. Camera Setup

- **Camera Mode:** Orthographic
- **Size:** 8-10 arası (ekran büyüklüğüne göre)
- **Background:** Uygun bir arka plan rengi

### 9. Layer Setup

1. Edit > Project Settings > Tags and Layers
2. Yeni layer ekle: **"Sliceable"**
3. Tüm meyve ve bomba prefablarını bu layer'a ata

## Oyunu Başlatma

1. `GameManager` GameObject'ini seç
2. Play mode'da Inspector'dan `StartGame()` metodunu çağır
3. Veya bir Button'a `OnClick` event olarak `FruitNinjaManager.StartGame` ekle

## Visual Effects (Opsiyonel)

### Particle Effect Ekleme

1. Slice efekti için particle system oluştur:
   - Beyaz, hızlı burst
   - Lifetime: 0.2-0.5 saniye
2. `SliceableObject` inspector'ında **Slice Effect Prefab** olarak ata

### Trail Renderer İyileştirme

1. `SliceController` GameObject'ine manuel `LineRenderer` ekle
2. Material:
   - Shader: Sprites/Default veya Particles/Additive
   - Color: Beyaz, parlak
   - Width: 0.1
3. Inspector'da `SliceController` referansına ata

## Test

1. Play mode'a gir
2. `GameManager.StartGame()` çağır
3. Mouse ile meyveler üzerinde swipe yap
4. Score ve timer'ın güncellendiğini kontrol et
5. Bombalara dokunma!

## Sorun Giderme

### Meyveler kesilmiyor
- Meyvelerin "Sliceable" layer'ında olduğunu kontrol et
- SliceController'ın "Sliceable Layer" ayarını kontrol et
- Collider2D'lerin aktif olduğunu kontrol et

### Trail görünmüyor
- LineRenderer component'inin aktif olduğunu kontrol et
- Material'in atandığını kontrol et
- Z-position'ın doğru olduğunu kontrol et

### Meyveler spawn olmuyor
- Prefabların atandığını kontrol et
- Spawner'ın `Initialize()` veya `StartSpawning()` çağrıldığını kontrol et
- Spawn pozisyonlarının kamera view'ında olduğunu kontrol et
