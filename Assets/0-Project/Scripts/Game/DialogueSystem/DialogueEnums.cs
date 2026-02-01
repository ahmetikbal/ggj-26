using System;

/// <summary>
/// Tüm konuşabilir karakterlerin enum tanımı
/// </summary>
public enum CharacterType
{
    Dedektif,
    AsciFadime,
    Garson,
    BesteciRedif,
    SimyaciSimurg,
    AycaHanim,
    TuccarAtlas,
    BeatriceHanim
}

/// <summary>
/// Diyalog node'u tamamlandığında çalıştırılacak aksiyonlar
/// </summary>
public enum DialogueAction
{
    None,
    
    // Minigame Actions
    StartMinigame_FruitNinja,
    StartMinigame_TableClean,
    
    // Zorunlu Karakter Geçişleri
    ForceCharacter_Garson,
    ForceCharacter_Asci,
    ForceCharacter_Tuccar,
    ForceCharacter_Ayca,
    ForceCharacter_Simyaci,
    ForceCharacter_Besteci,
    ForceCharacter_Beatrice,
    
    // Chapter İşlemleri
    EndChapter1,
    EndChapter2,
    StartChapter2,
    
    // Final
    ShowFinalDecision,
    
    // Karakter Durumu
    DisableCharacter_Besteci,  // Besteci ağlıyor, konuşamaz
    EnableCharacter_Besteci,   // Tüccar sonrası konuşabilir
    DisableCharacter_Simyaci,  // Simyacı meşgul
    EnableCharacter_Simyaci
}

/// <summary>
/// Oyunun genel chapter durumu
/// </summary>
public enum ChapterState
{
    Intro,
    Chapter1_AsciIntro,
    Chapter1_SalonFree,
    Chapter1_GarsonMinigame,
    Chapter1_AsciSecond,
    Chapter2_Start,
    Chapter2_SalonFree,
    Chapter2_FinalQuestion,
    Finale
}
