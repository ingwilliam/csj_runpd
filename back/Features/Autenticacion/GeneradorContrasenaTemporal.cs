using System.Security.Cryptography;

namespace Runpd.Api.Features.Autenticacion;

/// <summary>
/// Genera contrase\u00f1as temporales criptogr\u00e1ficamente seguras para la recuperaci\u00f3n de acceso.
/// </summary>
public sealed class GeneradorContrasenaTemporal
{
    private const int Longitud = 12;

    // Se omiten I, O, l, 0 y 1 para evitar confusiones al leer la contrase\u00f1a en el correo.
    private const string Mayusculas = "ABCDEFGHJKLMNPQRSTUVWXYZ";
    private const string Minusculas = "abcdefghijkmnopqrstuvwxyz";
    private const string Numeros = "23456789";
    private const string Simbolos = "!@#$%&*+-=?";
    private const string TodosLosCaracteres = Mayusculas + Minusculas + Numeros + Simbolos;

    /// <summary>
    /// Genera una contrase\u00f1a de 12 caracteres con may\u00fasculas, min\u00fasculas, n\u00fameros y s\u00edmbolos.
    /// </summary>
    /// <returns>Contrase\u00f1a temporal en texto plano. Solo debe enviarse por correo y guardarse como hash.</returns>
    public string Generar()
    {
        var caracteres = new List<char>(Longitud);

        // Paso 1: un car\u00e1cter de cada grupo garantiza que la contrase\u00f1a cumpla la complejidad exigida.
        caracteres.Add(ElegirCaracter(Mayusculas));
        caracteres.Add(ElegirCaracter(Minusculas));
        caracteres.Add(ElegirCaracter(Numeros));
        caracteres.Add(ElegirCaracter(Simbolos));

        // Paso 2: completar la longitud con caracteres de cualquier grupo.
        while (caracteres.Count < Longitud)
        {
            caracteres.Add(ElegirCaracter(TodosLosCaracteres));
        }

        // Paso 3: mezclar para que los caracteres obligatorios no queden siempre al inicio.
        Mezclar(caracteres);
        return new string(caracteres.ToArray());
    }

    /// <summary>
    /// Elige un car\u00e1cter aleatorio del grupo indicado.
    /// </summary>
    private static char ElegirCaracter(string grupo)
    {
        return grupo[RandomNumberGenerator.GetInt32(grupo.Length)];
    }

    /// <summary>
    /// Reordena los caracteres con el algoritmo Fisher-Yates usando aleatoriedad criptogr\u00e1fica.
    /// </summary>
    private static void Mezclar(List<char> caracteres)
    {
        for (var posicion = caracteres.Count - 1; posicion > 0; posicion--)
        {
            var posicionAleatoria = RandomNumberGenerator.GetInt32(posicion + 1);
            (caracteres[posicion], caracteres[posicionAleatoria]) =
                (caracteres[posicionAleatoria], caracteres[posicion]);
        }
    }
}
