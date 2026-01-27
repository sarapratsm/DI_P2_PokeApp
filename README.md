PokeUI - WPF Pokédex

🚀 Funcionalitats Destacades
1. Gestió de Pokédex (Vista Principal)
Cerca Dinàmica: Filtratge en temps real per nom o ID del Pokémon.

Filtre per Tipus: ComboBox amb indicador visual de colors segons el tipus seleccionat.

Navegació Fluida: Sistema de navegació integrat (Frame) entre la llista i l'edició.

Eliminació Robusta (Safe Delete): Implementació d'un sistema d'eliminació manual que gestiona les Foreign Keys. Elimina netament Tipus, Habilitats, Estadístiques i desvincula Evolucions abans d'esborrar el Pokémon per evitar errors d'integritat a la base de dades.

2. Editor de Pokémon (Detall)
Validació de Dades: Feedback visual (bordes vermells) i bloqueig del guardat si falten camps obligatoris.

Sistema d'Evolucions: Lògica personalitzada per gestionar cadenes evolutives per etapes (Stage 1 → Stage 2 → Stage 3), evitant bucles lògics i permetent reordenar o eliminar evolucions fàcilment.

Estadístiques Base: Edició visual mitjançant el UserControl personalitzat SlidingStatBar, amb TwoWay Binding per a HP, Atac, Defensa, etc.

Gestió d'Habilitats: Addició dinàmica d'habilitats amb control de duplicats.

🛠️ Stack Tecnològic
Framework: .NET 9.0

UI: Windows Presentation Foundation (WPF) + XAML

ORM: Entity Framework Core

Base de Dades: SQL Server / LocalDB

Patrons: MVVM (parcial), Data Binding, Dependency Properties.

🔧 Instal·lació i Execució
Clonar el repositori:

Configuració de Base de Dades:

Verifica la cadena de connexió a PokeData/AppDbContext.cs.

Aplica les migracions d'Entity Framework si és necessari:

Executar:

Obre la solució a Visual Studio 2022.

Estableix PokeUI com a projecte d'inici.

Compila i executa amb F5.
