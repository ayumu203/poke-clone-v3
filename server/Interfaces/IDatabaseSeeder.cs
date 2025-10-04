namespace server.Interfaces
{
    public interface IDatabaseSeeder
    {
        Task LoadPokemonSpeciesToDbAsync();
        Task LoadMovesToDbAsync();
        Task LoadPokemonMovesToDbAsync();
    }
}
