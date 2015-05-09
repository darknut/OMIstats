Olimpiada
	char numero key /*El objetivo es que sea 1,2, pero es char para poder contar la OMI intermedia y poder ponerle 8b o algo así*/
	char ciudad
	char estado /*No hay necesidad de poner la clave, podemos poner el estado completo*/
	int año
	date inicio
	date fin

Estado
	char clave key /* MEX, MDF, GTO, etc */
	char nombre
	char sitio /* Sitio de entrenamiento o sitio oficial de la olimpiada estatal en caso de existir */
		
MiembroDelegacion
	char olimpiada foreign key
	char estado foreign key	
	char clave /*La clave del concursante en la OMI DIF-1, GTO-2, etc (si las encontramos)*/
	char tipo /*El tipo de miembro, Lider, Competidor, Delegado, Invitado*/
	int persona foreign key
	char instutucion /*nombre de la escuela/empresa de donde viene*/
	char nivel /*Nivel educativo del participante*/
	int medalla /* De 1 a 4 donde 1 es oro y 4 es nada, como son cuatro valores, no es necesario hacer otra tabla*/

Persona /*Como una persona puede participar en varias olimpiadas, esta tabla es necesaria para identificarlos*/
	int clave autoincrement key
	char nombre		
	date nacimiento /*Campo opcional*/
	char facebook
	char twitter
	char sitio /*Estos tres campos son opcionales, pero como no podemos andarle preguntando a todos cuales son sus sitios, debemos de hacer algo para
				 que ellos mismos los llenen. Tal vez poner un login? O que lo hagan bajo peticion? */
	
MiembroDelegacionIOI
	int año /*De la IOI*/
	char olimpiada foreign key /*De qué olimpiada vino*/
	int persona foreign key
	char clave /* La clave del concursante en la IOI */ 
	char tipo /*competidor, lider, sublider, invitado*/
	int medalla /*Igual que arriba*/
	char estado foreign key 
	int IOIid /*El id del concursante en la pagina de la IOI para poder enlazar ahi, pe, Nieves tiene 1983 porque su enlace en la pagina es http://ioi.eduardische.com/people/1983 */
	
DelegacionIOI
	int año	key
	char pais
	char ciudad
	int puntos
	int lugar
	decimal media /*El numero de veces que los puntos de la delegacion estan sobre la media */
	
ResultadoProblema
	int olimpiada
	int dia
	int problema
	char concursante
	int puntos
	
Problema
	char olimpiada foreign key
	int dia
	int numero /*El numero de problema por dia, de 1 a 4*/
	char nombre
	char URL /* Link a un ZIP con el problema */
	decimal media
	int perfectos
	int ceros 
	int mediana /*Si faltan estadísticas, las agregamos */