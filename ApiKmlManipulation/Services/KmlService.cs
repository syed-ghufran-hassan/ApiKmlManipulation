using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpKml.Dom;
using SharpKml.Engine;
using ApiKMLManipulation.Models;
using System.ComponentModel;

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
        /// <param name="filePath">Caminho para o arquivo KML.</param>
        /// <exception cref="FileNotFoundException"></exception>
        public KmlService(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("Caminho do arquivo KML n�o pode ser nulo ou vazio.", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Arquivo KML n�o encontrado: {filePath}");
            }

            _filePath = filePath;
        }

        /// <summary>
        /// L� e parseia o arquivo KML, extraindo os dados relevantes.
        /// </summary>
        /// <returns>Lista de objetos <see cref="PlacemarkModel"/>.</returns>
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

                if (kmlFile.Root is Kml kml && kml.Feature is SharpKml.Dom.Container container)
                {
                    ExtractPlacemarksFromContainer(container, placemarks);
                }
                else
                {
                    throw new InvalidDataException("O arquivo KML n�o cont�m um container v�lido.");
                }
            }

            return placemarks;
        }

        /// <summary>
        /// Processa recursivamente containers e extrai placemarks.
        /// </summary>
        /// <param name="container">Container que pode conter placemarks ou outros containers.</param>
        /// <param name="placemarks">Lista de placemarks extra�dos.</param>
        private void ExtractPlacemarksFromContainer(SharpKml.Dom.Container container, List<PlacemarkModel> placemarks)
        {
            foreach (var feature in container.Features)
            {
                if (feature is Placemark placemark)
                {
                    var extendedData = placemark.ExtendedData;
                    var point = placemark.Geometry as Point;
                    if (extendedData != null)
                    {
                        var model = new PlacemarkModel
                        {
                            Cliente = GetDataValue(extendedData, "CLIENTE"),
                            Situacao = GetDataValue(extendedData, "SITUA��O"),
                            Bairro = GetDataValue(extendedData, "BAIRRO"),
                            Referencia = GetDataValue(extendedData, "REFERENCIA"),
                            RuaCruzamento = GetDataValue(extendedData, "RUA/CRUZAMENTO"),
                            Latitude = point.Coordinate.Latitude,
                            Longitude = point.Coordinate.Longitude
                        };

                        placemarks.Add(model);
                    }
                }
                else if (feature is SharpKml.Dom.Container childContainer)
                {
                    ExtractPlacemarksFromContainer(childContainer, placemarks);
                }
            }
        }

        /// <summary>
        /// Obt�m o valor de um campo espec�fico no <see cref="ExtendedData"/>.
        /// </summary>
        /// <param name="data">Dados estendidos associados a um placemark.</param>
        /// <param name="key">Chave do campo a ser extra�do.</param>
        /// <returns>Valor do campo, ou <c>null</c> se n�o encontrado.</returns>
        private string GetDataValue(ExtendedData data, string key)
        {
            return data?.Data?.FirstOrDefault(d => d.Name == key)?.Value?.Trim()?.Replace("\u00A0", " ");
        }
    }
}
