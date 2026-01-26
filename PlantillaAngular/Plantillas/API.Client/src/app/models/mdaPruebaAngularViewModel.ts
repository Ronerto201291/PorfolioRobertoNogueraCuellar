

export interface ComentariosTarea  {
    IdComentarioTarea: number;
    IdTarea: number;
    IdUsuarioComentario: number;
    FechaComentario: string;
    Comentario: string;
    Tarea: Tareas;
    Usuario: UsuariosPrueba;
}
export interface Tareas  {
    IdTarea: number;
    IdUsuarioTarea: number;
    NombreTarea: string;
    DescripcionTarea: string;
    FechaCreacion: string;
    FechaVencimiento: string | null;
    ComentariosTareas: ComentariosTarea[];
    Usuario: UsuariosPrueba;
}

export interface UsuariosPrueba {
    IdUsuario: number;
    NombreUsuario: string;
    Nombre: string;
    Apellidos: string;
    Email: string;
    Tareas: Tareas[];
    Posts: Post[];
    Blogs: Blog[];
    ComentariosTareas: ComentariosTarea[];
}

export interface Post  {
    IdPost: number;
    IdBlogPost: number;
    IdUsuarioPost: number;
    TituloPost: string;
    ContenidoPost: string;
    FechaPost: string;
    Blog: Blog;
    Usuario: UsuariosPrueba;
}

export interface Blog  {
    IdBlog: number;
    IdUsuarioOwner: number;
    Titulo: string;
    Posts: Post[];
    UsuarioOwner: UsuariosPrueba;
}
