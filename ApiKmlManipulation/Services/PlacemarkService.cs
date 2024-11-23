using System.Linq;
using ApiKMLManipulation.Models;
using System.Collections.Generic;

namespace ApiKMLManipulation.Services
{
    public class PlacemarkService
    {
        private readonly KmlService _kmlService;

        public PlacemarkService(KmlService kmlService)
        {
            _kmlService = kmlService;
        }

        /// <summary>
        /// Obt�m os valores �nicos de CLIENTE, SITUA��O e BAIRRO a partir dos placemarks extra�dos.
        /// </summary>
        /// <returns>Um dicion�rio com listas de valores �nicos para cada campo.</returns>
        public Dictionary<string, IEnumerable<string>> GetUniqueFilterValues()
        {
            var placemarks = _kmlService.ExtractPlacemarks();

            var clientes = placemarks
                .Select(p => p.Cliente)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct();

            var situacoes = placemarks
                .Select(p => p.Situacao)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct();

            var bairros = placemarks
                .Select(p => p.Bairro)
                .Where(b => !string.IsNullOrWhiteSpace(b))
                .Distinct();


            return new Dictionary<string, IEnumerable<string>>
            {
                { "Clientes", clientes },
                { "Situacoes", situacoes },
                { "Bairros", bairros }
            };
        }
    }
}
