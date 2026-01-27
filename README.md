# PokeApp 📱
**PokeApp** és una aplicació d'escriptori desenvolupada en **WPF (.NET 9)** per a la gestió integral d'una PokeDex. El projecte ha estat dissenyat seguint els requeriments de les unitats formatives RA2 i RA3, enfocant-se en la creació d'interfícies complexes, controls personalitzats i persistència de dades amb **MySQL**.

![Status](https://img.shields.io/badge/Status-Completed-success)
![Technology](https://img.shields.io/badge/.NET-9.0-purple)
![Platform](https://img.shields.io/badge/Platform-WPF-blue)
![Database](https://img.shields.io/badge/Database-MySQL-orange)

## 🚀 Funcionalitats Destacades

### 1. Gestió i Navegació
* **Cerca Avançada:** Filtratge dinàmic de Pokémon per nom o ID en temps real.
* **Navegació per Frames:** Sistema de navegació fluid entre la llista principal i el formulari de detalls.
* **Filtre de Tipus:** Selector personalitzat que utilitza el color corporatiu de cada tipus (codis HEX) definit a la base de dades.

### 2. Editor de Detalls i Controls Personalitzats
* **UserControl `SlidingStatBar`:** Control personalitzat amb *Dependency Properties* per visualitzar i editar les estadístiques base (HP, Atac, etc.) mitjançant un lliscador.
* **Càlcul de Febleses:** Lògica implementada per mostrar automàticament les debilitats i resistències segons el tipus del Pokémon seleccionat.
* **Gestió d'Evolucions:** Sistema per gestionar cadenes evolutives lineals (fins a 3 etapes), controlant que no es produeixin bucles i mantenint l'ordre lògic.
* **Validació d'Entrada:** Implementació de feedback visual (estils per a camps obligatoris) per garantir la integritat de les dades abans de desar.

### 3. Persistència i Seguretat de Dades
* **Esborrat Robust (Safe Delete):** Sistema manual que gestiona l'eliminació en cascada de les claus estrangeres (habilitats, tipus i estadístiques) per evitar errors de referència a MySQL.
* **ORM:** Ús d'**Entity Framework Core** amb el proveïdor Pomelo per a una comunicació eficient amb la base de dades.

---

## 🛠️ Stack Tecnològic

| Component | Tecnologia |
| :--- | :--- |
| **Framework** | .NET 9.0 |
| **UI** | WPF + XAML |
| **ORM** | Entity Framework Core (Pomelo) |
| **Base de Dades** | MySQL / MariaDB |
| **Arquitectura** | Patró MVVM (parcial) i Controls Personalitzats |

---

## 🔧 Instal·lació

1. **Base de Dades:**
   * Importa el fitxer `dump-pokedex-original.sql` al teu servidor MySQL.
   * *Nota:* El script inclou l'alteració de la taula `types` per afegir la columna `color`.
2. **Configuració:**
   * Actualitza la cadena de connexió a `AppDbContext.cs` amb les teves credencials de MySQL.
3. **Execució:**
   * Obre el projecte a **Visual Studio 2022**.
   * Compila i executa (`F5`).

---

## ✒️ Autor

**Sara Prats Morales**
* [LinkedIn](https://www.linkedin.com/in/sara-prats-morales)
* [GitHub](https://github.com/kuro-spm)
