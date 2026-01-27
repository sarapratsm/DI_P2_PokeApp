# 🚀 Funcionalidades Destacadas

### 1. Gestión de Pokédex (Vista Principal)
* **Búsqueda Dinámica:** Filtrado en tiempo real por nombre o ID del Pokémon.
* **Filtro por Tipos:** `ComboBox` con indicador visual de colores según el tipo seleccionado.
* **Navegación Fluida:** Sistema de navegación integrado (`Frame`) entre la lista y la edición.
* **Borrado Robusto (Safe Delete):** Implementación de un sistema de eliminación manual que gestiona las **Foreign Keys**. Elimina limpiamente Tipos, Habilidades, Estadísticas y desvincula Evoluciones antes de borrar el Pokémon para evitar errores de integridad en la base de datos.

### 2. Editor de Pokémon (Detalle)
* **Validación de Datos:** Feedback visual (bordes rojos) y bloqueo de guardado si faltan campos obligatorios.
* **Sistema de Evoluciones:** Lógica personalizada para gestionar cadenas evolutivas por etapas (Stage 1 → Stage 2 → Stage 3), evitando bucles lógicos y permitiendo reordenar o eliminar evoluciones.
* **Estadísticas Base:** Edición visual mediante el UserControl personalizado `SlidingStatBar`, con *TwoWay Binding* para HP, Ataque, Defensa, etc.
* **Gestión de Habilidades:** Adición dinámica de habilidades con control de duplicados.

---

## 🛠️ Stack Tecnológico

| Componente | Tecnología |
| :--- | :--- |
| **Framework** | .NET 9.0 |
| **UI** | WPF (Windows Presentation Foundation) + XAML |
| **ORM** | Entity Framework Core |
| **Base de Datos** | SQL Server / LocalDB |
| **Patrones** | MVVM (parcial), Data Binding, Dependency Properties |

---

## 🔧 Instalación y Ejecución

### 1. Clonar el repositorio
```bash
git clone [https://github.com/tu-usuario/tu-repositorio.git](https://github.com/tu-usuario/tu-repositorio.git)
