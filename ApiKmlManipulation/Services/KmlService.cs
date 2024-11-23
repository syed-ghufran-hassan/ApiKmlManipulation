using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpKml.Dom;
using SharpKml.Engine;
using ApiKMLManipulation.Models;

namespace ApiKMLManipulation.Services
{
    /// <summary>
    /// Servi�o respons�vel por manipular e processar arquivos KML.
    /// </summary>
    public class KmlService
    {
        private readonly string _filePath;

        /// <summary>
        /// Inicializa uma nova inst�ncia de <see cref="KmlService"/>.
        /// </summary>
        /// <param name="configuration">.</param>
        /// <exception cref="FileNotFoundException"></exception>
        public KmlService(IConfiguration configuration)
        {
            _filePath = Path.Combine(Directory.GetCurrentDirectory(), configuration["KmlFilePath"]);

            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"Arquivo KML n�o encontrado no caminho: {_filePath}");
            }
        }

        /// <summary>
        /// L� e parseia o arquivo KML, extraindo os dados relevantes.
        /// </summary>
        /// <returns>Lista de objetos <see cref="PlacemarkModel"/></returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        public List<PlacemarkModel> ExtractPlacemarks()
        {
            var placemarks = new List<PlacemarkModel>();

            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException("Arquivo KML n�o encontrado.", _filePath);
            }

            using (var fileStream = File.OpenRead(_filePath))
            {
                var kmlFile = KmlFile.Load(fileStream);

                if (kmlFile.Root is Kml kml && kml.Feature is Document document)
                {
                    foreach (var placemark in document.Features.OfType<Placemark>())
                    {
                        var extendedData = placemark.ExtendedData;
                        if (extendedData != null)
                        {
                            var model = new PlacemarkModel
                            {
                                Cliente = GetDataValue(extendedData, "CLIENTE"),
                                Situacao = GetDataValue(extendedData, "SITUA��O"),
                                Bairro = GetDataValue(extendedData, "BAIRRO"),
                                Referencia = GetDataValue(extendedData, "REFERENCIA"),
                                RuaCruzamento = GetDataValue(extendedData, "RUA/CRUZAMENTO")
                            };

                            placemarks.Add(model);
                        }
                    }
                }
                else
                {
                    throw new InvalidDataException("O arquivo KML n�o cont�m um documento v�lido.");
                }
            }

            return placemarks;
        }

        /// <summary>
        /// Obt�m o valor de um campo espec�fico no <see cref="ExtendedData"/>.
        /// </summary>
        /// <param name="data">Dados estendidos associados a um placemark.</param>
        /// <param name="key">Chave do campo a ser extra�do.</param>
        /// <returns>Valor do campo, ou <c>null</c> se n�o encontrado.</returns>
        private string GetDataValue(ExtendedData data, string key)
        {
            return data.Data.FirstOrDefault(d => d.Name == key)?.Value;
        }
    }
}
