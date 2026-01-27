PokeApp
=======
PokeApp és una aplicació d'escriptori WPF (.NET 9) dissenyada per a l'administració completa d'una base de dades Pokémon. El projecte implementa una arquitectura robusta que separa la lògica de dades de la interfície, oferint funcionalitats avançades com la gestió lineal d'evolucions, controls d'estadístiques personalitzats i un sistema d'eliminació segura per garantir la integritat referencial.

![Status](https://img.shields.io/badge/Status-Completed-success)
![Technology](https://img.shields.io/badge/.NET-9.0-purple)
![Platform](https://img.shields.io/badge/Platform-WPF-blue)
![Database](https://img.shields.io/badge/Database-MySql-red)

## 🚀 Funcionalitats Destacades

### 1. Gestió de Pokédex (Vista Principal)
* **Cerca Dinàmica:** Filtre en temps real per nom o ID del Pokémon.
* **Filtre per Tipus:** `ComboBox` amb indicador visual de colors segons el tipus seleccionat.
* **Navegació Fluida:** Sistema de navegació integrat (`Frame`) entre la llista i l'edició.
* **Esborrat Robust (Safe Delete):** Implementació d'un sistema d'eliminació manual que gestiona les **Foreign Keys**. Elimina netament Tipus, Habilitats, Estadístiques i desvincula Evolucions abans d'esborrar el Pokémon per evitar errors d'integritat a la base de dades.

### 2. Editor de Pokémon (Detall)
* **Validació de Dades:** Feedback visual (vores vermelles) i bloqueig de desat si falten camps obligatoris.
* **Sistema d'Evolucions:** Lògica personalitzada per gestionar cadenes evolutives per etapes (Stage 1 → Stage 2 → Stage 3), evitant bucles lògics i permetent reordenar o eliminar evolucions.
* **Estadístiques Base:** Edició visual mitjançant l'UserControl personalitzat `SlidingStatBar`, amb *TwoWay Binding* per a HP, Atac, Defensa, etc.
* **Gestió d'Habilitats:** Addició dinàmica d'habilitats amb control de duplicats.

---

## 🛠️ Stack Tecnològic

| Component | Tecnologia |
| :--- | :--- |
| **Framework** | .NET 9.0 |
| **UI** | WPF (Windows Presentation Foundation) + XAML |
| **ORM** | Entity Framework Core |
| **Base de Dades** | SQL Server / LocalDB |
| **Patrons** | MVVM (parcial), Data Binding, Dependency Properties |

---

## ✒️ Autor

**Sara Prats Morales**
* [LinkedIn](https://www.linkedin.com/in/sara-prats-morales)
* [GitHub](https://github.com/kuro-spm)
