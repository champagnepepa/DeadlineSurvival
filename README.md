#  Deadline Survival - 3D First-Person Survival Horror Game

![Engine](https://img.shields.io/badge/Engine-Unity_3D-blue?style=for-the-badge&logo=unity)
![Language](https://img.shields.io/badge/Language-C%23-green?style=for-the-badge&logo=csharp)
![AI Framework](https://img.shields.io/badge/AI-Finite_State_Machine-orange?style=for-the-badge)
![Genre](https://img.shields.io/badge/Genre-Survival_Horror-red?style=for-the-badge)

**Deadline Survival** adalah game *3D First-Person Survival Horror* yang dikembangkan menggunakan Unity Engine dan bahasa pemrograman C#. Game ini menceritakan tentang seorang karyawan yang terjebak di dalam gedung perkantoran dan harus bertahan hidup serta melarikan diri dari kejaran musuh (zombie). Proyek ini berfokus pada implementasi kecerdasan buatan (*Artificial Intelligence*) berbasis **Finite State Machine (FSM)** serta mekanisme *survival* seperti *inventory management*, *crafting*, dan *melee combo combat system*.

[![Deadline Survival Official Trailer](https://img.youtube.com/vi/Dh4W5Y8yaaY/maxresdefault.jpg)](https://youtu.be/Dh4W5Y8yaaY)
---

##  Screenshot Gameplay

<img width="600" height="450" alt="Screenshot 2026-07-23 171926" src="https://github.com/user-attachments/assets/9cf0b412-f16f-494e-9ddf-297d85bbcc44" />
<img width="600" height="450" alt="Screenshot 2026-07-23 171638" src="https://github.com/user-attachments/assets/dbe0fce0-b2b8-4a6a-af88-352196c64eb1" />
<img width="600" height="450" alt="Screenshot 2026-07-23 171615" src="https://github.com/user-attachments/assets/fb24b8b6-cb8d-4741-850b-7c7c373e63fe" />
<img width="600" height="450" alt="Screenshot 2026-07-23 171714" src="https://github.com/user-attachments/assets/2f1499f5-fefa-453f-a9b6-4a997336362b" />
<img width="600" height="450" alt="Screenshot 2026-07-23 171643" src="https://github.com/user-attachments/assets/2d0bb80d-0acf-4b68-99a7-08b8163088a0" />

---

##  Fitur & Sistem Utama Game

###  1. Enemy AI (Finite State Machine / FSM)
Kecerdasan buatan musuh (Zombie) dikelola menggunakan pendekatan **Finite State Machine (FSM)** yang terintegrasi dengan **Unity NavMesh Agent** untuk navigasi lingkungan. FSM ini terbagi menjadi 5 *state* utama:
-  **Idle State:** Musuh berdiam diri sejenak di titik lokasi tertentu.
-  **Patrol State:** Musuh bergerak secara otomatis menyusuri titik-titik koordinat (*waypoints*) yang ditentukan.
-  **Chase State:** Musuh mendeteksi dan mengejar pemain jika pemain memasuki radius deteksi ($< 3.0\text{f}$).
-  **Attack State:** Musuh mengeksekusi serangan saat jangkauan pemain sangat dekat ($< 1.5\text{f}$).
-  **Death State:** Musuh mati ketika nilai *health* mencapai 0 dan memicu *item drop* (*loot*).

---

###  2. Player Mechanics & Combat System
Pemain dilengkapi dengan kontrol sudut pandang orang pertama (*First-Person*) yang responsif serta sistem pertarungan jarak dekat (*melee attack*):
-  **Movement & Stamina System (`PlayerMovement.cs`):** Mengatur pergerakan *Walk* dan *Run/Sprint*. Penggunaan *Run* akan mengurangi tingkat dahaga/stamina (*Thirst Drain Rate*).
-  **Camera Look (`MouseLook.cs`):** Mengatur kontrol kamera FPS dengan batasan rotasi vertikal (*Clamped Pitch*) dan dukungan status *pause/interrupted*.
-  **Combo Attack System (`PlayerAttack.cs`):** Mekanisme serangan jarak dekat hingga 3 tahapan *combo* (`hit1`, `hit2`, `hit3`) berdasarkan waktu penekanan tombol (*click timing*) dan *cooldown*. Memiliki deteksi benturan senjata (*Weapon Collision Detection*) dan efek suara tebasan (*Swing Sound Effects*).
-  **World & Door Interaction (`PlayerInteractor.cs`):** Sistem pendeteksi interaksi tombol pintu (`DoorButtonTrigger`) dan item pintu (`DoorItem`) dalam radius *Sphere Overlap*.

---

###  3. Inventory & Item Interaction System
Sistem manajemen inventaris terintegrasi yang memudahkan pemain dalam mengumpulkan dan mengelola barang bertahan hidup:
-  **Raycast Pickup & Hover Outline (`PlayerInteraction.cs`):** Pemain dapat mengarahkan kursor ke objek di *world*. Objek yang dapat diambil akan memicu efek *Outline Shader* dan menampilkan nama item di UI.
-  **Interactive Audio:** Efek suara saat mengambil item (*pickup sound*) dengan variasi *pitch* dinamis.
-  **Hand Attachment & Equip Sync:** Sinkronisasi visual item yang dipegang di tangan (*Hand Parent*) secara otomatis meng-update animasi *Equip* pada karakter.

---

### 4. Crafting System
Sistem pembuatan item (*crafting*) yang memungkinkan pemain menggabungkan berbagai bahan baku (*raw materials*) dari *inventory* untuk menciptakan item baru, seperti senjata bertahan hidup yang lebih bagus lagi dari durabilitas sampai damage yang dihasilkan.

---

## Struktur Script Utama (Core Scripts)

| Script Name | Deskripsi Fungsi |
| :--- | :--- |
| `PlayerMovement.cs` | Mengontrol pergerakan fisik pemain, efek suara langkah (*footsteps*), gravitasi, serta kalkulasi pengurasan stamina. |
| `PlayerAttack.cs` | Mengatur rantai animasi combo serangan, *cooldown*, dan pemicu efek suara ayunan senjata. |
| `MouseLook.cs` | Memproses *input* kursor mouse untuk rotasi pandangan kamera First-Person. |
| `PlayerInteraction.cs` | Deteksi *raycast* kursor ke item dunia, efek *outline*, dan pemicu fungsi penyimpanan inventaris. |
| `PlayerInteractor.cs` | Deteksi interaksi area (seperti tombol, pemicu pintu, dan objek lingkungan). |

---

## Tech Stack & Tools

- **Game Engine:** Unity 3D Engine
- **Programming Language:** C# (.NET / Mono)
- **Navigation & Pathfinding:** Unity NavMesh Agent
- **UI Framework:** Unity TextMeshPro (TMP)
- **Code Editor:** Visual Studio Code / Visual Studio 2022

---

## Penulis / Author

* **Nama:** Muhammad Khansa Syahrizal Azra
