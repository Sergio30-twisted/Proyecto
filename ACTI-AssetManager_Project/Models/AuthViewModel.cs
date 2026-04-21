namespace ACTI_AssetManager_Project.Models
{
    public class AuthViewModel
    {

        /// <summary>
        /// El Model contiene los datos 
        /// que la View va a mostrar, editar o capturar usando asp-for
        /// </summary>
        public string Username { get; set; }
        public string Password { get; set; }

        //El Controller crea/obtiene el Model y el mismo controller se lo pasa a la View, y el View lo usa
        //Request → Controller → obtiene/crea Model → View(Model) → HTML

    }
}
