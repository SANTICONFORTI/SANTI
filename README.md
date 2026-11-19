# SANTI – PPIII

| Carpeta | Qué es | Cómo ejecutar |
|---|---|---|
| `Parcial2/` | Login/logout + permisos (Composite + Singleton), WinForms en capas | Abrir `Parcial2.sln`, proyecto de inicio **UI**. Usuarios: `admin`/`123`, `vendedor`/`123` |
| `Parcial3/` | YaLlega: tarifas (Strategy), catálogo (Proxy), exportador genérico, en capas | Abrir `Parcial3.sln`, proyecto de inicio **UI** (consola con casos de prueba) |
| `Parcial4/` | Impresora concurrente (Singleton + Monitor + Productor-Consumidor + notificaciones) | Abrir `Parcial4.sln` |
| `Kit_Recu/` | Mismos ejercicios en un solo archivo cada uno + MACHETE | – |
| `KitRecu/` | Solución con todos los ejercicios juntos | – |

Capas (Parcial 2 y 3): **DomainModel** (entidades) · **DAL** (datos) · **BLL** (negocio) · **Services** (transversales) · **UI**.
Referencias: UI → BLL → DAL, y todas → DomainModel.
