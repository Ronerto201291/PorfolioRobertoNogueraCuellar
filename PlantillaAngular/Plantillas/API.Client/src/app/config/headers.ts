export const headers = [
    {
      label: 'Area de trabajo',
      url: 'areaTrabajo'
    },
    {
      label: 'Portal Colaborativo',
      url: 'portalColaborativo'
    },
    {
      label: 'Gestor de Incidencias',
      url: 'gestorIncidencias'
    },
    {
      label: 'Personas',
      url: 'personas/busquedaPersonas',
      children: [
        {
          label: 'Busqueda de personas',
          url: 'personas/busquedaPersonas',
        }
      ]

    },
    {
      label: 'Motor de comunicaciones',
      url: 'motorComuniaciones'
    },
    {
      label: 'Prestaciones',
      url: 'prestaciones'
    },
    {
      label: 'Suscripción',
      url: 'suscripcion'
    },
]