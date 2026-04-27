# Hidden Valley 

Este proyecto es una aplicación web completa que utiliza una arquitectura de microservicios contenerizada con Docker. Incluye una API robusta y un frontend interactivo desarrollado en Blazor. Es un proyecto universitario para el curso de Analisi de Sistemas. Se basa en una aplicacion para llevar el control y registros de cabañas y visitantes de un turicentro llamado Hidden Valley ubicado en Monjas, Jalapa, Guatemala

## Tecnologías Utilizadas

* **Backend:** .NET 9 Web API
* **Frontend:** Blazor WebAssembly / Server
* **Base de Datos:** SQL Server (Entity Framework Core)
* **Contenedores:** Docker & Docker Compose
* **Documentación de la API:** Swagger (Swashbuckle)

## Requisitos Previos

Antes de comenzar, asegúrate de tener instalado:
* [Docker Desktop](https://www.docker.com/products/docker-desktop/)
* [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
* [Entity Framework Core Tools](https://learn.microsoft.com/en-us/ef/core/get-started/dotnet-cli) (`dotnet tool install --global dotnet-ef`)

## Instalación y Configuración

### 1. Clonar el repositorio

bash
git clone [https://github.com/tu-usuario/HiddenValley.git](https://github.com/tu-usuario/HiddenValley.git)
cd HiddenValley

### 2 Comandos de Desarrollo (opcional)
Si deseas replicar la instalación de dependencias o actualizar los paquetes, estos son los comandos utilizados:

dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0

#### Documentacion de API en swagger
dotnet add package Swashbuckle.AspNetCore

### 3 Levantar contenedor de docker

Este comando descarga las imágenes, compila los proyectos y levanta los servicios:

* docker-compose up --build

### Puntos de acceso

Página Web (Frontend): http://localhost:7075

Documentación API (Swagger): http://localhost:7084/swagger

Servidor de Base de Datos: localhost,1433

## Autores
- Jorge Danilo Ucelo
- Christian Eduardo Lopez Lemus
- Delmi Maria Fajardo
- Keily Atalia Lopez Hernandez
- Cristian Eduardo Chamo Morales

ESTUDIANTES DE INGENIERIA EN SISTEMAS DE LA UNIVERSIDAD MARIANO GALVEZ DE GUATEMALA SEDE EN JALAPA,JALAPA
