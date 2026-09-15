# Worklog - Card Battle Game Development
## วันที่: 14 กันยายน 2569

---

## สรุปงาน

### ปัญหาเริ่มต้น
- ผู้ใช้มีปัญหา Instagram ทำให้เครื่อง重启 อัตโนมัติบน Infinix Hot 10
- แนะนำให้ใช้ Instagram Lite แทน

### ขอคำปรึกษาเกี่ยวกับ Mobile Game Development
- ผู้สนใจสร้างเกมมือถือ类型 Gwent Card Battle
- เลือกใช้ Unity engine

---

## ไฟล์ที่สร้าง

### โครงสร้างโปรเจค
```
CardBattle/
└── Assets/
    └── Scripts/
        ├── AI/
        │   └── AIPlayer.cs
        ├── Battle/
        │   ├── BattleManager.cs
        │   ├── BattleUIManager.cs
        │   ├── BoardUI.cs
        │   ├── GameManager.cs
        │   ├── HandUI.cs
        │   └── TurnManager.cs
        ├── Card/
        │   ├── CardData.cs
        │   ├── CardInstance.cs
        │   └── CardUI.cs
        ├── Deck/
        │   ├── DeckData.cs
        │   └── DeckManager.cs
        └── SETUP_GUIDE.txt
```

---

## รายละเอียดไฟล์

### Card System

**CardData.cs**
- ScriptableObject สำหรับเก็บข้อมูลการ์ด
- Fields: cardName, description, artwork, power, cost, faction, cardType, ability

**CardInstance.cs**
- Runtime instance ของ card
- เก็บ currentPower, isOnBoard, isRevealed

**CardUI.cs**
- UI component สำหรับแสดงผลการ์ด
- รองรับ drag & drop

### Battle System

**BattleManager.cs**
- จัดการ battle flow ทั้งหมด
- ควบคุม round, score, turn

**TurnManager.cs**
- จัดการ turn-based system
-  mana system, turn timer

**BoardUI.cs**
- แสดงผล board (melee, ranged, siege rows)
- คำนวณ score

**HandUI.cs**
- แสดงผล cards ในมือ
- จัดการ layout

### AI System

**AIPlayer.cs**
- AI opponent logic
- เลือกการ์ดที่ดีที่สุดจาก playable cards

### Deck System

**DeckManager.cs**
- จัดการ deck, hand, graveyard
- Shuffle, draw cards

**DeckData.cs**
- เก็บ deck configuration

### Game Management

**GameManager.cs**
- Game state management (MainMenu, Battle, DeckBuilder, Victory, Defeat)

**BattleUIManager.cs**
- UI management สำหรับ battle scene

---

## คุณสมบัติของเกม

### Core Features
1. Turn-based card battle system
2. Mana system (1 card = 1 mana)
3. 3 card rows: Melee, Ranged, Siege
4. Card abilities: Bond, Spy, Medic, Muster, Morale, Scorch

### Battle Flow
1. Start battle - draw 10 cards
2. Player turn -> Enemy turn
3. Play cards from hand to board
4. Calculate scores
5. Best of 3 rounds

### Card Abilities
- **Bond**: Power x2 when multiple same cards
- **Medic**: Return card from graveyard
- **Spy**: Give card to opponent, draw 2 cards
- **Morale**: Boost adjacent cards
- **Scorch**: Destroy strongest card

---

## วิธีตั้งค่าใน Unity

1. สร้าง Unity project (2D Core)
2. คัดลอกไฟล์ .cs ทั้งหมดไป Assets/Scripts/
3. สร้าง Card prefab
4. สร้าง Card Data (ScriptableObject)
5. สร้าง Battle scene
6. ลิงค์ components ใน Inspector

ดูรายละเอียดใน SETUP_GUIDE.txt

---

## ปัญหาที่พบ

- ไม่มีปัญหาในการสร้างโค้ด
- ต้อง setup Unity project ด้วยตนเอง

---

## แผนงานถัดไป

- สร้าง Unity project
- Import assets (art, sound)
- สร้าง card database
- ทำ UI/UX design
- ทดสอบ gameplay
- ปรับ balance

---

## บันทึก

- โค้ดสร้างเสร็จวันที่ 14 กันยายน 2569
- ไฟล์打包เป็น CardBattle_project.tar.gz (7.0 KB)
- พร้อมใช้งานใน Unity

---

*Worklog created by opencode*
