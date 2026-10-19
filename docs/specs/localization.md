# Spec: play the game in Spanish, Portuguese and Indonesian

Status: proposal for after 1.2.0 (PR #2) is merged. Written 2026-10-19.

Labels: **[code]** from the repo, **[review data]** from the Play Console material in the product audit, **[inferred]** not seen on a device, **[decision]** my proposal.

## 1. Findings

1. **The game has been English-only since 2020.** `StartUpManager` always loads `localizedText_en.json`; the system-language switch is commented out (commit 7083c06, 2020-02-29) **[code]**.
2. **The Spanish file is a placeholder.** On master it has 3 entries, one of them `game_title = "Optimov"` (another project's name) **[code]**. PR #2 adds Spanish for its 22 new strings but not the 22 original ones.
3. **The font has no accented letters.** `Assets/Fonts/spacecow.ttf` ("space cowboy.") covers printable ASCII plus `œ` only; á é í ó ú ñ ¿ ¡ ã õ ç are missing, and the importer has no fallback font **[code: checked with fontTools]**. Unity would draw the missing glyphs from an OS font, in a different style, or not at all **[inferred; needs a device check]**.
4. **A few texts bypass localisation:** "Level N" in the HUD (`LevelDisplay`), "Coming Soon!" (Choose Level), "Congratulations!" (Win Screen) **[code]**.
5. **The audience is not English-first.** Spanish is the largest review group; English, Indonesian and Portuguese follow; devices are mostly low-end **[review data, audit section 8]**. One 2021 review complained the text was not localised.

## 2. Plan [decision]

1. **Font:** keep "space cowboy" for its look and add a fallback font with Latin Extended (an SIL OFL font from Google Fonts with a similar rounded, bold style) via the font importer's fallback list, so accented letters render in a consistent style. Also check the licence of "space cowboy" for commercial use; nothing in the repo records it **[open]**.
2. **Language choice:** pick the file from `Application.systemLanguage`: Spanish → `es`, Portuguese → `pt`, Indonesian → `id`, anything else → `en`. Add a language row on the Options screen later only if analytics or reviews ask for it.
3. **Strings:** complete `es`, add `pt` (Brazilian Portuguese wording) and `id`, and add three keys for the hard-coded texts (`level_label`, `coming_soon`, `congratulations`). Keys PR #2 adds for leaderboards (`leaderboard`, `leaderboards`, `win_rank`) are unused since leaderboards left 1.2.0; keep them out of new files.
4. **Missing-key safety:** a missing key falls back to the English value, never to "Localized text not found" (PR #2's `UiText.Get` already does this for its strings; extend it to `LocalizedText`).
5. **Store listing:** the listing translations are a separate task (see `/mnt/project-files/audit/store-listing-draft.md` in the project files); ship them together so a Spanish listing leads to a Spanish game.

Vietnamese (also in reviews) needs many stacked diacritics and a font that covers them; leave it until the three languages above show results.

## 3. Translations

Drafts by Claude, written to read naturally rather than literally. Spanish rows that PR #2 already contains are reused unchanged. **A native speaker should read the `help_text` and tutorial lines before release** (marked †).

| Key | English | Spanish (es) | Portuguese (pt-BR) | Indonesian (id) |
|---|---|---|---|---|
| game_title | Plants vs Animals | Plants vs Animals | Plants vs Animals | Plants vs Animals |
| loading_text | Loading... | Cargando... | Carregando... | Memuat... |
| main_start | Start Game | Jugar | Jogar | Mulai |
| main_options | Options | Opciones | Opções | Pengaturan |
| main_help | Help | Ayuda | Ajuda | Bantuan |
| main_quit | Quit | Salir | Sair | Keluar |
| options_options | Options | Opciones | Opções | Pengaturan |
| options_volume | Volume | Volumen | Volume | Volume |
| options_difficulty | Difficulty | Dificultad | Dificuldade | Kesulitan |
| options_default | Default | Predeterminado | Padrão | Default |
| options_back | Back | Volver | Voltar | Kembali |
| help_heading | The story: | La historia: | A história: | Ceritanya: |
| help_back | Back | Volver | Voltar | Kembali |
| help_text † | (unchanged) | ¡Los animales quieren comerse las plantas! Pero las plantas no están indefensas: cada una tiene sus propias armas. Usa tus estrellas con cuidado y coloca plantas en el campo para defenderte. Las setas te dan estrellas extra con el tiempo, y no solo eso... Cada vez que un animal llega al final del campo, pierdes una vida. Sube la dificultad en Opciones para ganar más puntos. | Os animais querem comer as plantas! Mas as plantas não estão indefesas: cada uma tem suas próprias armas. Use suas estrelas com cuidado e coloque plantas no campo para revidar. Os cogumelos dão estrelas extras com o tempo, e não só isso... Cada vez que um animal chega ao fim do campo, você perde uma vida. Aumente a dificuldade em Opções para ganhar mais pontos. | Hewan-hewan ingin memakan tanaman! Tapi tanaman tidak tinggal diam: masing-masing punya senjata sendiri. Gunakan bintangmu dengan bijak dan tanam tanaman di lahan untuk melawan. Jamur memberimu bintang tambahan seiring waktu, dan bukan hanya itu... Setiap kali seekor hewan sampai di ujung lahan, kamu kehilangan satu nyawa. Naikkan kesulitan di Pengaturan untuk mendapat poin lebih banyak. |
| choose_heading | Choose Level: | Elige nivel: | Escolha a fase: | Pilih Level: |
| choose_level | Level | Nivel | Fase | Level |
| main_menu | Main Menu | Menú principal | Menu principal | Menu Utama |
| win_text | You Won! | ¡Has ganado! | Você venceu! | Kamu Menang! |
| try_again | Play Again | Jugar otra vez | Jogar de novo | Main Lagi |
| level_lost | That was close! | ¡Por poco! | Foi por pouco! | Hampir saja! |
| level_complete | Level Complete | ¡Nivel superado! | Fase concluída! | Level Selesai! |
| continue_text | Continue | Continuar | Continuar | Lanjut |
| pause_heading | Paused | Pausa | Pausado | Jeda |
| pause_resume | Resume | Seguir | Continuar | Lanjutkan |
| pause_restart | Restart | Reiniciar | Reiniciar | Ulangi |
| win_retry | Retry | Repetir | Tentar de novo | Coba Lagi |
| continue_ad | Continue (ad) | Continuar (anuncio) | Continuar (anúncio) | Lanjut (iklan) |
| continue_unavailable | No ad available right now | No hay anuncios ahora | Nenhum anúncio disponível agora | Iklan belum tersedia |
| tutorial_next | Next | Siguiente | Próximo | Berikutnya |
| tutorial_play | Play! | ¡A jugar! | Jogar! | Main! |
| privacy_options | Privacy | Privacidad | Privacidade | Privasi |
| cannot_plant | Plants can't grow here | Aquí no crecen plantas | Plantas não crescem aqui | Tanaman tak bisa tumbuh di sini |
| win_score | Score | Puntos | Pontos | Skor |
| win_new_best | New best! | ¡Nuevo récord! | Novo recorde! | Rekor baru! |
| win_best | Best | Récord | Recorde | Rekor |
| win_continued | Continued run: score not recorded | Partida continuada: puntos no registrados | Partida continuada: pontuação não registrada | Dilanjutkan: skor tidak dicatat |
| tutorial_1 † | Hungry animals are coming for your garden! Stop them before they reach the left side. | (from PR #2) | Animais famintos estão vindo para a sua horta! Pare-os antes que cheguem ao lado esquerdo. | Hewan-hewan lapar datang ke kebunmu! Hentikan mereka sebelum sampai ke sisi kiri. |
| tutorial_2 † | Pick a plant from the bar, then tap a square of the lawn to plant it. | (from PR #2) | Escolha uma planta na barra e toque em um quadrado do gramado para plantá-la. | Pilih tanaman dari bar, lalu ketuk kotak di rumput untuk menanamnya. |
| tutorial_3 † | Plants cost stars. Mushrooms make more stars, so plant some early. | (from PR #2) | Plantas custam estrelas. Cogumelos geram mais estrelas, então plante alguns logo no início. | Tanaman butuh bintang. Jamur menghasilkan bintang, jadi tanam beberapa sejak awal. |
| tutorial_4 † | Every animal that gets through costs a life. Hold on until the timer runs out to win. | (from PR #2) | Cada animal que passa custa uma vida. Aguente até o tempo acabar para vencer. | Setiap hewan yang lolos mengurangi satu nyawa. Bertahanlah sampai waktu habis untuk menang. |
| tutorial_5 † | Your score is the stars you have left at the end. Save as many as you can! | (from PR #2) | Sua pontuação são as estrelas que sobram no final. Guarde o máximo que puder! | Skormu adalah bintang yang tersisa di akhir. Simpan sebanyak mungkin! |
| level_label (new) | Level | Nivel | Fase | Level |
| coming_soon (new) | Coming Soon! | ¡Próximamente! | Em breve! | Segera Hadir! |
| congratulations (new) | Congratulations! | ¡Felicidades! | Parabéns! | Selamat! |

## 4. Code changes (after PR #2 is merged)

- `StartUpManager.cs`: choose the file from `Application.systemLanguage` (section 2.2).
- `LocalizationManager.cs` / `LocalizedText.cs`: load English first, then overlay the device language, so a missing key shows English and never a missing-text marker.
- `LevelDisplay.cs`: use `level_label`. Choose Level "Coming Soon!" and Win Screen "Congratulations!" get keys (these are not level scenes).
- `Assets/StreamingAssets/localizedText_{en,es,pt,id}.json`: one file per language, pretty-printed (one entry per line) so later edits do not all conflict on a single long line.
- Font importer: fallback font added; long strings checked for overflow (Portuguese and Indonesian run longer than English).

## 5. Acceptance criteria

- With the device set to Spanish, Portuguese or Indonesian, every screen (splash, start, options, help, choose level, a level with pause, tutorial, win, lose, continue) shows that language with correct accents in a style matching the rest of the UI, and never "Localized text not found".
- Any other device language shows English.
- No text overflows its box on a 16:9 phone, a 20:9 phone and Nikos's tablet.
- No level scene under `Assets/Scenes/Levels` changes.
- No new SDK, permission or data type.

## 6. Effort [estimate]

One session for code and font, one short session for device checks in three languages. Native review of the † lines is optional for a first release.
