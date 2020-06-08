# voicemodchat
App de chat local en .Net Core 3.1

# Inyección de dependencias
He utilizado el framework propio de .Net para la inyección de dependencias del servicio de chat.

# Arquitectura
Por la 'sencillez' de la aplicación (en cuanto a tamaño y funcionalidad) no he visto necesario crear una estructura de proyecto más compleja. En caso de aplicaciones más extensas, con más servicios, acceso a datos, etc, suelo separar cada capa (servicios, repositorios, front...) en distintos proyectos.

# Observaciones
No conocía la librería utilizada (Fleck). Pensaba que signalR era la más estándar para este tipo de notificaciones y habría sido la elegida por mi parte en caso de haber podido elegir libremente por tener más documentación y haberla utilizado anteriormente para envío de notificaciones.
