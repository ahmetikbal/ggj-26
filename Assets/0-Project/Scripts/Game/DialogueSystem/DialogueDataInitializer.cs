using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Oyun başlangıcında DialogueDatabase'i örnek verilerle doldurur
/// Bu script sadece test/development için kullanılır
/// Gerçek kullanımda DialogueDatabase ScriptableObject'i Inspector'dan doldurulur
/// </summary>
public class DialogueDataInitializer : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private DialogueDatabase targetDatabase;
    
    [Header("Settings")]
    [SerializeField] private bool initializeOnAwake = false;
    
    private void Awake()
    {
        if (initializeOnAwake && targetDatabase != null)
        {
            InitializeChapter1Data();
        }
    }
    
    [ContextMenu("Initialize Chapter 1 Data")]
    public void InitializeChapter1Data()
    {
        if (targetDatabase == null)
        {
            Debug.LogError("Target Database not assigned!");
            return;
        }
        
        targetDatabase.characters.Clear();
        
        // Aşçı Fadime
        CreateAsciDialogues();
        
        // Garson
        CreateGarsonDialogues();
        
        // Besteci Redif
        CreateBesteciDialogues();
        
        // Simyacı Simurg
        CreateSimyaciDialogues();
        
        // Ayça Hanım
        CreateAycaDialogues();
        
        // Tüccar Atlas
        CreateTuccarDialogues();
        
        // Beatrice Hanım
        CreateBeatriceDialogues();
        
        Debug.Log("[DialogueDataInitializer] Chapter 1 data initialized!");
        
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(targetDatabase);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }
    
    #region Aşçı Fadime
    
    private void CreateAsciDialogues()
    {
        var asciData = new CharacterDialogueData
        {
            characterType = CharacterType.AsciFadime,
            displayName = "Aşçı Fadime",
            dialogueNodes = new List<DialogueNode>()
        };
        
        // Entry node - İlk karşılaşma
        asciData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch1_entry",
            editorNote = "Aşçı ile ilk konuşma",
            lines = new DialogueLine[]
            {
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Aşçı hanım iyi günler, burda kayıp papağan davası üzerine soruşturma yapmak için bulunuyorum. Bilgileriniz bizim için kıymetli." },
                new DialogueLine { speaker = CharacterType.AsciFadime, text = "Anladık evladım. Sus bakiyim burda işimiz var. Konuşmak istiyosan önce işimi kolaylaştır." }
            },
            choices = new DialogueChoice[]
            {
                new DialogueChoice
                {
                    choiceText = "Kabul et",
                    actionOnSelect = DialogueAction.StartMinigame_FruitNinja,
                    flagToSet = "accepted_asci_help"
                }
            },
            nextNodeId = null,
            minimumChapter = 1
        });
        
        // Minigame sonrası
        asciData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch1_post_minigame",
            editorNote = "Fruit Ninja sonrası",
            requiredFlags = new string[] { "fruitninja_completed" },
            lines = new DialogueLine[]
            {
                new DialogueLine { speaker = CharacterType.AsciFadime, text = "İşte böyle. Ben vallahi kimseyi görmedim. Bukadar kişiye yemek yetiştiriyorum." },
                new DialogueLine { speaker = CharacterType.AsciFadime, text = "Ama şurdaki garson çok aylak aylak dolaşıyor ona sor o bilir." },
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Pekala. Teşekkür ederim hanımefendi." }
            },
            choices = new DialogueChoice[0],
            onCompleteAction = DialogueAction.ForceCharacter_Garson,
            minimumChapter = 1
        });
        
        // Chapter 1 sonu - Zorunlu geçiş
        asciData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch1_second",
            editorNote = "Chapter 1 sonu - Aşçıya tekrar dönüş",
            requiredFlags = new string[] { "tableclean_completed" },
            lines = new DialogueLine[]
            {
                new DialogueLine { speaker = CharacterType.AsciFadime, text = "Ahahah garson hep böyledir. Ona aldırış etme. Utangaç görünür ama rüşvet tutkunudur, bırakmaz!" },
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Simyacıyı gördün mü?" },
                new DialogueLine { speaker = CharacterType.AsciFadime, text = "Ah hayır. Çok meşguldum. Görmemiş olabilirim." },
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Hakkında ne biliyorsun?" },
                new DialogueLine { speaker = CharacterType.AsciFadime, text = "Eee, fazla şey değil. Sohbet ederiz arada. Yeni tedavilerden falan bahseder bana. Hep yeni şeyler dener, hele sihirli iksirler! Bazen ondan korkuyorum ahahah" },
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Diğerleri?" },
                new DialogueLine { speaker = CharacterType.AsciFadime, text = "Maalesef çoğu kişi sohbete gelmez. O çatlak garson bile benim halimi sormaz!" }
            },
            choices = new DialogueChoice[0],
            onCompleteAction = DialogueAction.EndChapter1,
            minimumChapter = 1
        });
        
        targetDatabase.characters.Add(asciData);
    }
    
    #endregion
    
    #region Garson
    
    private void CreateGarsonDialogues()
    {
        var garsonData = new CharacterDialogueData
        {
            characterType = CharacterType.Garson,
            displayName = "Garson",
            dialogueNodes = new List<DialogueNode>()
        };
        
        // Entry node
        garsonData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch1_entry",
            editorNote = "Garson ile ilk konuşma",
            lines = new DialogueLine[]
            {
                new DialogueLine { speaker = CharacterType.Garson, text = "Hoşgeldiniz hoşgeldiniz" },
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Teşekkürler, izninizle olay anında nerede ne yapıyor olduğunuzu sorabilir miyim?" },
                new DialogueLine { speaker = CharacterType.Garson, text = "Ta-tabi. Herzamanki gibi çalışıyordum. Mutfakla salon arası git gel işte" },
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Bu salondan olay anına yakın kimler çıktı?" },
                new DialogueLine { speaker = CharacterType.Garson, text = "Hmmm. Hatırlamak zor. Hanımefendi ev sahibinin eşi Ayça hanım, simyacı Simurg, besteci Redif, Tüccar Atlas ve aile dostu Beatrice hanım o civarlarda salondan ayrıldılar." },
                new DialogueLine { speaker = CharacterType.Garson, text = "Fakat nereye gittiklerini bilmiyorum ne yazık ki." },
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Çok teşekkürler." }
            },
            choices = new DialogueChoice[0],
            nextNodeId = null,
            minimumChapter = 1
        });
        
        // Chapter 1 - Minigame öncesi
        garsonData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch1_second",
            editorNote = "Garson - masa toplama minigame öncesi",
            requiredFlags = new string[] { "talked_all_suspects" },
            lines = new DialogueLine[]
            {
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Kimse doğru düzgün anlatmıyor. Bence sen daha fazla şey hatırlayabilirsin." },
                new DialogueLine { speaker = CharacterType.Garson, text = "Aslını istersen… sen bu masaları toplarken ben de biraz düşünebilirim" }
            },
            choices = new DialogueChoice[]
            {
                new DialogueChoice
                {
                    choiceText = "Kabul et",
                    actionOnSelect = DialogueAction.StartMinigame_TableClean
                }
            },
            minimumChapter = 1
        });
        
        // Minigame sonrası
        garsonData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch1_post_tableclean",
            editorNote = "Masa toplama sonrası",
            requiredFlags = new string[] { "tableclean_completed" },
            lines = new DialogueLine[]
            {
                new DialogueLine { speaker = CharacterType.Garson, text = "Sanırım kıyafetini siyah gibi gördüm. Hmm. Çok dikkat etmedim ama öyle gibiydi. Galiba ya" },
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Bu kadar mı?" },
                new DialogueLine { speaker = CharacterType.Garson, text = "Siyahtı evet. Başka bilmiyorum. Çalışıyorum ben!" }
            },
            choices = new DialogueChoice[0],
            onCompleteAction = DialogueAction.ForceCharacter_Asci,
            minimumChapter = 1
        });
        
        // Chapter 2 - Opsiyonel
        garsonData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch2_entry",
            editorNote = "Chapter 2 - Opsiyonel yardım",
            lines = new DialogueLine[]
            {
                new DialogueLine { speaker = CharacterType.Garson, text = "Yardım edersen sana bi iki bilgi daha vereyim" }
            },
            choices = new DialogueChoice[]
            {
                new DialogueChoice
                {
                    choiceText = "a) Elbette",
                    responseLines = new DialogueLine[]
                    {
                        new DialogueLine { speaker = CharacterType.Garson, text = "Kuşun gitmesi iyi oldu açıkçası. Eee ne konuşsan yanında hemen sahibine ötüyor!" },
                        new DialogueLine { speaker = CharacterType.Garson, text = "Maaş az diyorum ertesi gün maaş daha da azalıyor!! Hem bakamıyordu ev sahibi. Yıllardır uçmadı o kuş." },
                        new DialogueLine { speaker = CharacterType.Dedektif, text = "Bildiğin herşeyi anlat garson" },
                        new DialogueLine { speaker = CharacterType.Garson, text = "Hiçbişey! Hem paranız var mı bayım dedikodu istiyorsunuz!" }
                    },
                    flagToSet = "garson_extra_info"
                },
                new DialogueChoice
                {
                    choiceText = "b) Gerek yok teşekkürler",
                    responseLines = new DialogueLine[0]
                }
            },
            minimumChapter = 2
        });
        
        targetDatabase.characters.Add(garsonData);
    }
    
    #endregion
    
    #region Besteci Redif
    
    private void CreateBesteciDialogues()
    {
        var besteciData = new CharacterDialogueData
        {
            characterType = CharacterType.BesteciRedif,
            displayName = "Besteci Redif",
            dialogueNodes = new List<DialogueNode>()
        };
        
        // Entry node
        besteciData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch1_entry",
            editorNote = "Besteci ile ilk konuşma",
            lines = new DialogueLine[]
            {
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Merhaba Redif bey. Sizinle papağanın kayboluşu ile ilgili konuşmak istiyorum." }
            },
            choices = new DialogueChoice[]
            {
                new DialogueChoice
                {
                    choiceText = "a) Olay anında ne yapıyordunuz?",
                    responseLines = new DialogueLine[]
                    {
                        new DialogueLine { speaker = CharacterType.BesteciRedif, text = "Yan salonda ses çalışıyordum… Kalabalık ortamlarda çalışılmıyor." },
                        new DialogueLine { speaker = CharacterType.BesteciRedif, text = "Ben.. gerçekten çok üzüldüm. Onunla her günü beste çalışarak geçirirdim ona zarar vermek aklımdan bile geçmez..." }
                    },
                    flagToSet = "besteci_alibi_asked"
                },
                new DialogueChoice
                {
                    choiceText = "b) Papağanla yakınlığınız nedir?",
                    responseLines = new DialogueLine[]
                    {
                        new DialogueLine { speaker = CharacterType.BesteciRedif, text = "Her gün birlikte beste çalışırız, Çalışırdık. Çok güzel bestelerimiz olurdu." },
                        new DialogueLine { speaker = CharacterType.BesteciRedif, text = "Hatta kaybolduğu an yan salonda bestelerimizden birini çalışıyordum..." }
                    },
                    flagToSet = "besteci_relationship_asked"
                }
            },
            nextNodeId = "ch1_followup",
            minimumChapter = 1
        });
        
        // Takip soruları
        besteciData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch1_followup",
            editorNote = "Besteci - kapı sorusu",
            lines = new DialogueLine[]
            {
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Pekala bayım. Papağanın yanından ayrılırken kapısını kapattığınıza eminsiniz değil mi?" },
                new DialogueLine { speaker = CharacterType.BesteciRedif, text = "Elbette. Asla ihmal etmem." },
                new DialogueLine { speaker = CharacterType.Dedektif, text = "…Maalesef papağanla en çok vakit geçiren kişi olarak ifadeleriniz önemli, gitmenize izin veremeyiz." },
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Lütfen diğerlerinin ifadelerini alırken rahatlamaya çalışın bayım." }
            },
            choices = new DialogueChoice[0],
            onCompleteAction = DialogueAction.DisableCharacter_Besteci,
            minimumChapter = 1
        });
        
        // Chapter 2 - Tüccar sonrası aktif
        besteciData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch2_entry",
            editorNote = "Chapter 2 - Tüccar ile konuştuktan sonra",
            requiredFlags = new string[] { "talked_TuccarAtlas", "chapter_2_started" },
            blockedByFlags = new string[] { "besteci_unavailable" },
            lines = new DialogueLine[0],
            choices = new DialogueChoice[]
            {
                new DialogueChoice
                {
                    choiceText = "a) Tüye alerjiniz mi var bayım?",
                    responseLines = new DialogueLine[]
                    {
                        new DialogueLine { speaker = CharacterType.BesteciRedif, text = "Evet- neden? (Hapşu)" }
                    }
                },
                new DialogueChoice
                {
                    choiceText = "b) Kuş sesi besteleriniz için önemli olmalı",
                    responseLines = new DialogueLine[]
                    {
                        new DialogueLine { speaker = CharacterType.BesteciRedif, text = "Harmoniyi güzelleştiriyor (hapşu)" }
                    }
                }
            },
            nextNodeId = "ch2_confrontation",
            minimumChapter = 2
        });
        
        // Yüzleşme
        besteciData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch2_confrontation",
            editorNote = "Besteci ile yüzleşme",
            lines = new DialogueLine[]
            {
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Gözlüğünüzdeki tüyleri açıklar mısınız lütfen?" },
                new DialogueLine { speaker = CharacterType.BesteciRedif, text = "Ben, ben.. (ağlamaya başlar) Hayır düşündüğünüz gibi değil!" },
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Eminim her misafir baloya özenli gelirdi, kuş tüyleri kasıtlı değilse." },
                new DialogueLine { speaker = CharacterType.BesteciRedif, text = "Ben değilim! Değilim. Ben kaçırmadım (ağlar)" }
            },
            choices = new DialogueChoice[0],
            minimumChapter = 2
        });
        
        targetDatabase.characters.Add(besteciData);
    }
    
    #endregion
    
    #region Simyacı Simurg
    
    private void CreateSimyaciDialogues()
    {
        var simyaciData = new CharacterDialogueData
        {
            characterType = CharacterType.SimyaciSimurg,
            displayName = "Simyacı Simurg",
            dialogueNodes = new List<DialogueNode>()
        };
        
        // Entry node
        simyaciData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch1_entry",
            editorNote = "Simyacı ile ilk konuşma",
            lines = new DialogueLine[]
            {
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Papağanla alakalı düşünceleriniz neler bay Simurg?" },
                new DialogueLine { speaker = CharacterType.SimyaciSimurg, text = "Ehm. Çok değerli bi kuştu. Kaybı çok beklenmedikti. Türünün tek örneğiydi." },
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Peki olay anında neredeydiniz?" },
                new DialogueLine { speaker = CharacterType.SimyaciSimurg, text = "Ben, salondan çıktım. Fadime aşçıyla sohbet etmeye gidiyordum." },
                new DialogueLine { speaker = CharacterType.SimyaciSimurg, text = "Meşguldü, seslenmedim bende. Yukarıdan bi patırtı duydum." },
                new DialogueLine { speaker = CharacterType.SimyaciSimurg, text = "Belki biri düşmüştür diye gidip bakayım dedim ancak kimse yoktu. Bende anlam veremedim doğrusu." },
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Peki teşekkürler" }
            },
            choices = new DialogueChoice[0],
            minimumChapter = 1
        });
        
        // Chapter 2
        simyaciData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch2_entry",
            editorNote = "Chapter 2 - Simyacı sorgusu",
            lines = new DialogueLine[0],
            choices = new DialogueChoice[]
            {
                new DialogueChoice
                {
                    choiceText = "a) Papağan tüyü çalışmalarınız için değerli miydi?",
                    responseLines = new DialogueLine[]
                    {
                        new DialogueLine { speaker = CharacterType.SimyaciSimurg, text = "Hiçbirimiz simya uğruna eziyeti savunmayız." },
                        new DialogueLine { speaker = CharacterType.Dedektif, text = "Haklısınız, peki sizce neden bestecinin maskesinde tüy olsun? Ne işine yarayabilir." },
                        new DialogueLine { speaker = CharacterType.SimyaciSimurg, text = "Besteci mi? Hmm. Henüz ortadan kaybolmamış bi kuşun tüyünün aksesuar olmaması için dramatik bir neden yok." },
                        new DialogueLine { speaker = CharacterType.Dedektif, text = "Besteci değilse kim?" },
                        new DialogueLine { speaker = CharacterType.SimyaciSimurg, text = "Beatrice hanım onun eski sahibi… Onu zorla aldıklarında çok üzülmüştü. Gidin ona sorun!" },
                        new DialogueLine { speaker = CharacterType.Dedektif, text = "Peki ya deneyleriniz? O tüy çok faydalı olabilirdi…" },
                        new DialogueLine { speaker = CharacterType.SimyaciSimurg, text = "O başka bir mesele dedektif bey! Ben simyaya hizmet ediyorum hırsızlığa değil." }
                    },
                    flagToSet = "simyaci_accused_beatrice"
                },
                new DialogueChoice
                {
                    choiceText = "b) Size papağanın tüyünden versem, bana bildiklerinizi anlatır mısınız?",
                    responseLines = new DialogueLine[]
                    {
                        new DialogueLine { speaker = CharacterType.SimyaciSimurg, text = "Tüyü nasıl elde ettiniz? Çok yanlış anlıyorsunuz bayım." },
                        new DialogueLine { speaker = CharacterType.Dedektif, text = "Lütfen, simyanın önemini hepimiz biliyoruz." },
                        new DialogueLine { speaker = CharacterType.SimyaciSimurg, text = "(…) Pekala alayım." },
                        new DialogueLine { speaker = CharacterType.SimyaciSimurg, text = "Bugün Beatrice hanım hiç iyi gözükmüyordu. Gergindi. Buraya uzun zamandır ilk defa geliyor." },
                        new DialogueLine { speaker = CharacterType.SimyaciSimurg, text = "Elinden kuşunu zorla aldıklarından beri hiç gelmemişti! Kuşuna çok bağlıydı" },
                        new DialogueLine { speaker = CharacterType.Dedektif, text = "Kuş onda değil bundan eminiz, yine de değerlendireceğim. Teşekkür ederim." }
                    },
                    flagToSet = "simyaci_bribed"
                }
            },
            minimumChapter = 2
        });
        
        // Final soru hakkı
        simyaciData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch2_final",
            editorNote = "Son soru hakkı",
            requiredFlags = new string[] { "final_question_phase" },
            lines = new DialogueLine[]
            {
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Besteci son bir kez kendini savunacak mı?" },
                new DialogueLine { speaker = CharacterType.SimyaciSimurg, text = "Maalesef bayım. Kendine getiremiyoruz, sürekli ağlıyor." }
            },
            choices = new DialogueChoice[0],
            minimumChapter = 2
        });
        
        targetDatabase.characters.Add(simyaciData);
    }
    
    #endregion
    
    #region Ayça Hanım
    
    private void CreateAycaDialogues()
    {
        var aycaData = new CharacterDialogueData
        {
            characterType = CharacterType.AycaHanim,
            displayName = "Ayça Hanım",
            dialogueNodes = new List<DialogueNode>()
        };
        
        // Entry node
        aycaData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch1_entry",
            editorNote = "Ayça Hanım ile ilk konuşma",
            lines = new DialogueLine[]
            {
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Merhabalar Ayça hanım. Olan bitenle ilgili düşünceleriniz neler?" },
                new DialogueLine { speaker = CharacterType.AycaHanim, text = "Ah güzel papağanım Ah. Kimler kaçırdı seni!" },
                new DialogueLine { speaker = CharacterType.AycaHanim, text = "Ona hayran hayran bakardım. Sanki tüm güzellikler ondaydı. Eşim bile onu benden çok severdi!" },
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Olay sırasında ne yapıyordunuz?" },
                new DialogueLine { speaker = CharacterType.AycaHanim, text = "Bakın dedektif bey. Haber gelmeden bi süre önce hava almaya çıkmıştım. Misafirler beni çok boğuyor anlatamam." }
            },
            choices = new DialogueChoice[]
            {
                new DialogueChoice
                {
                    choiceText = "a) Dışarıda normal dışı birşey gördünüz mü?",
                    responseLines = new DialogueLine[]
                    {
                        new DialogueLine { speaker = CharacterType.AycaHanim, text = "Hayır, hayır görmedim. Ne olduysa ben çıkmadan oldu dışarıda hiçbişey görmedim." }
                    }
                },
                new DialogueChoice
                {
                    choiceText = "b) Eşiniz için kuşun değeri neydi?",
                    responseLines = new DialogueLine[]
                    {
                        new DialogueLine { speaker = CharacterType.AycaHanim, text = "Asil bir papağandı. Evine çok yakışıyordu ve çok akıllıydı." },
                        new DialogueLine { speaker = CharacterType.AycaHanim, text = "Gizemliydi ayrıca iyi bi miktar ödemişti onun için." }
                    }
                }
            },
            nextNodeId = "ch1_end",
            minimumChapter = 1
        });
        
        aycaData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch1_end",
            lines = new DialogueLine[]
            {
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Anlıyorum. Cevaplarınız için teşekkürler." }
            },
            choices = new DialogueChoice[0],
            minimumChapter = 1
        });
        
        // Chapter 2
        aycaData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch2_entry",
            editorNote = "Chapter 2 - Ayça sorgusu",
            lines = new DialogueLine[]
            {
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Papağanda özel bi durum var mıydı?" },
                new DialogueLine { speaker = CharacterType.AycaHanim, text = "Evet kesinlikle. Ne olduğunu bilmiyoruz ama kesinlikle farklıydı!" },
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Mesela?" },
                new DialogueLine { speaker = CharacterType.AycaHanim, text = "Sihir değilse bile her duyduğunu tekrar edebilirdi! Bazen de sinir bozucu tabii." }
            },
            choices = new DialogueChoice[]
            {
                new DialogueChoice
                {
                    choiceText = "a) Simyacı nasıl biri?",
                    responseLines = new DialogueLine[]
                    {
                        new DialogueLine { speaker = CharacterType.AycaHanim, text = "Garip. Fazla ketum. Çok da konuşmaz." }
                    }
                },
                new DialogueChoice
                {
                    choiceText = "b) Şüphelendiğiniz biri var mı?",
                    responseLines = new DialogueLine[]
                    {
                        new DialogueLine { speaker = CharacterType.AycaHanim, text = "Biri olsa simyacı derdim… Papağanı en çok merak eden oydu. Hep incelemek istiyordu." }
                    },
                    flagToSet = "ayca_suspects_simyaci"
                }
            },
            minimumChapter = 2
        });
        
        // Tüccar sonrası
        aycaData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch2_post_tuccar",
            editorNote = "Tüccar çağırdıktan sonra - Ayça sorgusu",
            requiredFlags = new string[] { "tuccar_alibi_ayca" },
            lines = new DialogueLine[0],
            choices = new DialogueChoice[]
            {
                new DialogueChoice
                {
                    choiceText = "a) Beyefendi size samimi yaklaşıyor sanırım.",
                    responseLines = new DialogueLine[]
                    {
                        new DialogueLine { speaker = CharacterType.AycaHanim, text = "Efendim? Atlas bey mi? Evet ve hoş bi insan." }
                    }
                },
                new DialogueChoice
                {
                    choiceText = "b) Beyefendi size çok yakın uğrar mı?",
                    responseLines = new DialogueLine[]
                    {
                        new DialogueLine { speaker = CharacterType.AycaHanim, text = "Atlas bey mi? Ah hayır. Maalesef. Ama geldiğinde hoş hediyeler getirir." }
                    }
                }
            },
            nextNodeId = "ch2_flower_talk",
            minimumChapter = 2
        });
        
        aycaData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch2_flower_talk",
            lines = new DialogueLine[]
            {
                new DialogueLine { speaker = CharacterType.AycaHanim, text = "Bir keresinde mor menekşe getirmişti. Bu binanın meşalelerinin kızıl ışıklarıyla okadar hoş duruyor ki…" },
                new DialogueLine { speaker = CharacterType.AycaHanim, text = "Oysa heryerde unutmabeni çiçeği var karaltı gibi sanki…" }
            },
            choices = new DialogueChoice[]
            {
                new DialogueChoice
                {
                    choiceText = "a) Hava almaya Atlas beyle mi çıktınız?",
                    responseLines = new DialogueLine[]
                    {
                        new DialogueLine { speaker = CharacterType.AycaHanim, text = "Ben- evet elbette yalnız sessiz olun dedektif rica ediyorum. Yerin kulağı var!" },
                        new DialogueLine { speaker = CharacterType.Dedektif, text = "Pekala" }
                    },
                    flagToSet = "ayca_with_atlas_confirmed"
                },
                new DialogueChoice
                {
                    choiceText = "b) Hava almaya tek mi çıktınız?",
                    responseLines = new DialogueLine[]
                    {
                        new DialogueLine { speaker = CharacterType.AycaHanim, text = "Evet, yani hayır. Pek sayılmaz, neden önemli ki bu?" },
                        new DialogueLine { speaker = CharacterType.Dedektif, text = "Sizi gören birinin şahit olması açısından" },
                        new DialogueLine { speaker = CharacterType.AycaHanim, text = "Ah elbette. Atlas bey görmüştür, ona sorabilirsiniz. Dediğim gibi dışarıdaydım." },
                        new DialogueLine { speaker = CharacterType.Dedektif, text = "Yeterli, teşekkürler" }
                    }
                }
            },
            minimumChapter = 2
        });
        
        // Final soru hakkı
        aycaData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch2_final",
            requiredFlags = new string[] { "final_question_phase" },
            lines = new DialogueLine[]
            {
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Genelde bayımla buluşmalarınız nasıl geçer?" },
                new DialogueLine { speaker = CharacterType.AycaHanim, text = "Genelde sessiz. Çok konuşmayız… Nerede yaşıyor bilmem bile." }
            },
            choices = new DialogueChoice[0],
            minimumChapter = 2
        });
        
        targetDatabase.characters.Add(aycaData);
    }
    
    #endregion
    
    #region Tüccar Atlas
    
    private void CreateTuccarDialogues()
    {
        var tuccarData = new CharacterDialogueData
        {
            characterType = CharacterType.TuccarAtlas,
            displayName = "Tüccar Atlas",
            dialogueNodes = new List<DialogueNode>()
        };
        
        // Entry node
        tuccarData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch1_entry",
            editorNote = "Tüccar ile ilk konuşma",
            lines = new DialogueLine[]
            {
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Papağan kaybolduğunda neredeydiniz bay Atlas." },
                new DialogueLine { speaker = CharacterType.TuccarAtlas, text = "Ben mi? Ben bir iş adamıyım. Her saniye işim var. Kafes kuşlarıyla uğraşamam." },
                new DialogueLine { speaker = CharacterType.TuccarAtlas, text = "Balo da tam bi zaman kaybıydı. Normalde asla gelmezdim fakat bu sefer özel bi davet oldu." },
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Nasıl bir özel davet bayım?" },
                new DialogueLine { speaker = CharacterType.TuccarAtlas, text = "Bu sizi hiç ilgilendirmez dedektif. Siz işinize bakın." }
            },
            choices = new DialogueChoice[0],
            minimumChapter = 1
        });
        
        // Chapter 2
        tuccarData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch2_entry",
            editorNote = "Chapter 2 - Tüccar sorgusu",
            lines = new DialogueLine[]
            {
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Ev sahibiyle hiç sorununuz oldu mu?" },
                new DialogueLine { speaker = CharacterType.TuccarAtlas, text = "Hayır. Asla o adiyi ciddiye almam ben." }
            },
            choices = new DialogueChoice[]
            {
                new DialogueChoice
                {
                    choiceText = "a) Neden öyle düşünüyorsunuz?",
                    responseLines = new DialogueLine[]
                    {
                        new DialogueLine { speaker = CharacterType.TuccarAtlas, text = "Kişisel fikrim. Ayrıca dedektiflik yeteneklerinizi sorgulamalısınız." },
                        new DialogueLine { speaker = CharacterType.TuccarAtlas, text = "Bestecinin maskesine dikkat ettiniz mi hiç?" }
                    },
                    flagToSet = "tuccar_hint_besteci"
                },
                new DialogueChoice
                {
                    choiceText = "b) Anladım.",
                    responseLines = new DialogueLine[]
                    {
                        new DialogueLine { speaker = CharacterType.TuccarAtlas, text = "Ha birde. Bestecinin maskesine dikkat edin derim!" },
                        new DialogueLine { speaker = CharacterType.TuccarAtlas, text = "Birtakım noktaları kaçırıyor olabilirsin dedektif." }
                    },
                    flagToSet = "tuccar_hint_besteci"
                }
            },
            onCompleteAction = DialogueAction.EnableCharacter_Besteci,
            minimumChapter = 2
        });
        
        // Tüccar çağırır (zorunlu)
        tuccarData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch2_alibi",
            editorNote = "Tüccar Ayça için şahitlik yapar",
            requiredFlags = new string[] { "talked_all_ch2" },
            lines = new DialogueLine[]
            {
                new DialogueLine { speaker = CharacterType.TuccarAtlas, text = "Ayça hanım benimleydi. Ondan şüphe etmiyorsunuzdur umarım" }
            },
            choices = new DialogueChoice[]
            {
                new DialogueChoice
                {
                    choiceText = "a) Hayır herşey normal gözüküyor şuan için",
                    responseLines = new DialogueLine[]
                    {
                        new DialogueLine { speaker = CharacterType.TuccarAtlas, text = "Elbette, ben adamımla konuşmaya gittiğimde bana eşlik etmişti. Kafese gittiği falan yok" },
                        new DialogueLine { speaker = CharacterType.Dedektif, text = "Hoş şahitliğiniz için teşekkürler. İşimin başına dönmeliyim." }
                    },
                    flagToSet = "tuccar_alibi_ayca"
                },
                new DialogueChoice
                {
                    choiceText = "b) Sizden de şüphe ediyorum bayım",
                    responseLines = new DialogueLine[]
                    {
                        new DialogueLine { speaker = CharacterType.TuccarAtlas, text = "İnanın bana dedektif bu işin içindeki kişiyi bulmaya bende sizin kadar hevesliyim." },
                        new DialogueLine { speaker = CharacterType.TuccarAtlas, text = "Ayça hanımı rahat bırakın ben de size yardım edeyim diyorum size!" },
                        new DialogueLine { speaker = CharacterType.Dedektif, text = "Maalesef dava çözülene kadar kimse burdan çıkamayacak. Siz ve Ayça hanım da dahil." }
                    },
                    flagToSet = "tuccar_alibi_ayca"
                }
            },
            minimumChapter = 2
        });
        
        // Final soru hakkı
        tuccarData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch2_final",
            requiredFlags = new string[] { "final_question_phase" },
            lines = new DialogueLine[]
            {
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Papağanın ticari değeri ne kadar?" },
                new DialogueLine { speaker = CharacterType.TuccarAtlas, text = "Bu devirde kime sihirli papağan yutturursan okadar. Kimseye!" }
            },
            choices = new DialogueChoice[0],
            minimumChapter = 2
        });
        
        targetDatabase.characters.Add(tuccarData);
    }
    
    #endregion
    
    #region Beatrice Hanım
    
    private void CreateBeatriceDialogues()
    {
        var beatriceData = new CharacterDialogueData
        {
            characterType = CharacterType.BeatriceHanim,
            displayName = "Beatrice Hanım",
            dialogueNodes = new List<DialogueNode>()
        };
        
        // Entry node
        beatriceData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch1_entry",
            editorNote = "Beatrice ile ilk konuşma",
            lines = new DialogueLine[]
            {
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Merhaba hanımefendi." },
                new DialogueLine { speaker = CharacterType.BeatriceHanim, text = "Merhabalar…" }
            },
            choices = new DialogueChoice[]
            {
                new DialogueChoice
                {
                    choiceText = "a) Neler olduğunu anlatabilir misiniz?",
                    responseLines = new DialogueLine[]
                    {
                        new DialogueLine { speaker = CharacterType.BeatriceHanim, text = "Evet elbette. Nasıl kaybolabilir anlamıyorum." },
                        new DialogueLine { speaker = CharacterType.BeatriceHanim, text = "Olay anında lavaboya gitmiştim. Göz açıp kapayıncaya kadar felaket!" },
                        new DialogueLine { speaker = CharacterType.BeatriceHanim, text = "Benim babam da hayvanlarla ilgileniyor aslında şuan burda olsa arardık!.." }
                    }
                },
                new DialogueChoice
                {
                    choiceText = "b) Aileyle bağlantınızdan bahseder misiniz?",
                    responseLines = new DialogueLine[]
                    {
                        new DialogueLine { speaker = CharacterType.BeatriceHanim, text = "Hmm. Babam ev sahibiyle çok eskiden beri tanışır." },
                        new DialogueLine { speaker = CharacterType.BeatriceHanim, text = "Babam hayvanlarla ilgilenir ve bu ailenin tüm hayvanlarına bakmıştır. Bu kadar aslında." },
                        new DialogueLine { speaker = CharacterType.BeatriceHanim, text = "Size olay anından bahsedeyim ben. Resmen lavabodaydım!" },
                        new DialogueLine { speaker = CharacterType.BeatriceHanim, text = "Kuş kayıp dediler. İnanamadım. Bulursanız haber verin lütfen" },
                        new DialogueLine { speaker = CharacterType.Dedektif, text = "Elbette hanımefendi" }
                    }
                }
            },
            nextNodeId = "ch1_end",
            minimumChapter = 1
        });
        
        beatriceData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch1_end",
            lines = new DialogueLine[]
            {
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Cevaplarınız için teşekkürler" }
            },
            choices = new DialogueChoice[0],
            minimumChapter = 1
        });
        
        // Chapter 2
        beatriceData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch2_entry",
            editorNote = "Chapter 2 - Beatrice sorgusu",
            lines = new DialogueLine[0],
            choices = new DialogueChoice[]
            {
                new DialogueChoice
                {
                    choiceText = "a) Papağan sizin için önemli miydi?",
                    responseLines = new DialogueLine[]
                    {
                        new DialogueLine { speaker = CharacterType.BeatriceHanim, text = "Aslında evet. Çok hatırlamak istemiyorum, üzgünüm." }
                    }
                },
                new DialogueChoice
                {
                    choiceText = "b) Besteci iyi yalancı mıdır?",
                    responseLines = new DialogueLine[]
                    {
                        new DialogueLine { speaker = CharacterType.BeatriceHanim, text = "Ben onun daha önce yalan söylediğine şahit olmadım…" }
                    }
                }
            },
            minimumChapter = 2
        });
        
        // Simyacı accusation sonrası
        beatriceData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch2_confrontation",
            editorNote = "Simyacı suçladıktan sonra",
            requiredFlags = new string[] { "simyaci_accused_beatrice" },
            lines = new DialogueLine[]
            {
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Sizin için eski kuşunuz çok severdi diyorlar…" },
                new DialogueLine { speaker = CharacterType.BeatriceHanim, text = "Simyacı mı söyledi onu? O pislik!" },
                new DialogueLine { speaker = CharacterType.BeatriceHanim, text = "Beni arkamdan bıçaklamadan önce sırdaşımdı…" },
                new DialogueLine { speaker = CharacterType.BeatriceHanim, text = "Kuşumu aldıklarında bidaha buraya gelmemeye söz verdim ama Simurg bana ihanet edip sürekli buraya gelmeye başladı!" },
                new DialogueLine { speaker = CharacterType.BeatriceHanim, text = "Alçak herif. Bana da bidaha gelmedi. Sırf kuş için…" }
            },
            choices = new DialogueChoice[]
            {
                new DialogueChoice
                {
                    choiceText = "a) Papağandan faydalandığını mı söylüyorsunuz?",
                    responseLines = new DialogueLine[]
                    {
                        new DialogueLine { speaker = CharacterType.BeatriceHanim, text = "Evet! Bakın, her sevgi dürüst değilmiş. Anlıyor musunuz?" }
                    },
                    flagToSet = "beatrice_accuses_simyaci"
                },
                new DialogueChoice
                {
                    choiceText = "b) Siz papağanı geri almak istemediniz yani.",
                    responseLines = new DialogueLine[]
                    {
                        new DialogueLine { speaker = CharacterType.BeatriceHanim, text = "Hayır! Öyle bi durum yok. HANİ KUŞ?. Yok." },
                        new DialogueLine { speaker = CharacterType.BeatriceHanim, text = "Zavallı kuş.. Hiç değilse artık o sefil kafeste acı çekmiyordur!" },
                        new DialogueLine { speaker = CharacterType.BeatriceHanim, text = "Sevmek hapsetmek değil bayım!" }
                    },
                    flagToSet = "beatrice_motive_revealed"
                }
            },
            nextNodeId = "ch2_end",
            minimumChapter = 2
        });
        
        beatriceData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch2_end",
            lines = new DialogueLine[]
            {
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Anlıyorum, biraz dinlenin." }
            },
            choices = new DialogueChoice[0],
            minimumChapter = 2
        });
        
        // Final soru hakkı
        beatriceData.dialogueNodes.Add(new DialogueNode
        {
            nodeId = "ch2_final",
            requiredFlags = new string[] { "final_question_phase" },
            lines = new DialogueLine[]
            {
                new DialogueLine { speaker = CharacterType.Dedektif, text = "Beatrice hanım eski arkadaşınız simyacı papağan kanı ister miydi?" },
                new DialogueLine { speaker = CharacterType.BeatriceHanim, text = "Eski Simurg kan akıtmazdı. Söyleyebileceğim bu kadar." }
            },
            choices = new DialogueChoice[0],
            minimumChapter = 2
        });
        
        targetDatabase.characters.Add(beatriceData);
    }
    
    #endregion
}
