﻿using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class ControladorJuego : MonoBehaviour
{
    public static ControladorJuego instancia = null;
    [SerializeField] private ControladorMapa mapa;

    private List<Unidad> ejercito;
    private Vector3 posMundo;
    private Unidad unidadElegida;
    private bool seleccionandoTile = false;

    void Awake()
    {
        // se asegura que solo exista una instancia del controlador de juego
        if (instancia == null)
            instancia = this;
        else if (instancia != this)
            Destroy(gameObject);

        ejercito = new List<Unidad>();

        mapa = GetComponent<ControladorMapa>();
        IniciarJuego();
    }

    void IniciarJuego()
    {
        // carga el mapa, la grilla, los obstaculos, las unidades, etc
        mapa.CrearEscenario();
    }

    public void AgregarUnidad(Unidad componenteScript)
    {
        ejercito.Add(componenteScript);
    }

    void Update()
    {
        // @TODO: crear y manejar las zonas de despliegue

        if (unidadElegida == null)
        {
            foreach (Unidad unidad in ejercito)
            {
                if (unidad.SeSelecciono())
                {
                    unidadElegida = unidad;
                    break;
                }
            }
        }
        
        if ((unidadElegida != null))
        {
            if(!unidadElegida.EstaMoviendo())
                SeleccionarTile(unidadElegida);

            // mueve la unidad si debe hacerlo
            unidadElegida.Mover(posMundo);

            if (!unidadElegida.EstaSeleccionada() && !unidadElegida.EstaMoviendo())
                unidadElegida = null;
        }
    }

    void SeleccionarTile(Unidad unidad)
    {
        if (Input.GetMouseButtonDown(0))
        {
            // obtiene la posicion del mouse dentro del juego
            Vector3 posMouseMundo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            bool clickEnGrilla = mapa.ObtenerPosGrilla(posMouseMundo, out int tileX, out int tileY);

            if (clickEnGrilla)
            {
                // obtiene el tile(x, y) en el que esta la unidad
                mapa.ObtenerPosGrilla(unidad.ObtenerPosicion(), out int tileUnidadX, out int tileUnidadY);

                unidad.DeterminarRadioTiles(tileUnidadX, tileUnidadY);
                unidad.CambiarTilesMovimiento(mapa.DeterminarTilesInvalidos(unidad.ObtenerTilesMovimiento(), tileUnidadX, tileUnidadY, unidad.ObtenerRadioMovimiento()));
                unidad.CambiarTilesAtaque(mapa.DeterminarTilesAtaque(unidad.ObtenerTilesMovimiento(), tileUnidadX, tileUnidadY));
                List<Vector2> tilesMovimiento = unidad.ObtenerTilesMovimiento();
                List<Vector2> tilesAtaque = unidad.ObtenerTilesAtaque();

                // comprueba que el click no haya sido sobre el tile de la unidad
                if (tileX != tileUnidadX || tileY != tileUnidadY)
                {
                    if (unidad.ClickEnTileMovimiento(tileX, tileY))
                    {
                        // obtiene la posicion del tile a la que mover
                        posMundo = mapa.ObtenerPosMundo(tileX, tileY);
                        unidad.DeterminarDireccionMovimiento(posMundo);
                    }
                    else
                        unidad.ToggleSeleccion(false);
                    
                    mapa.DestruirTiles();
                    tilesMovimiento.Clear();
                    tilesAtaque.Clear();
                    seleccionandoTile = false;
                }
                else if (!seleccionandoTile)
                {
                    // crea los tiles de movimiento
                    mapa.InstanciarTilesMovimiento(tilesMovimiento);
                    mapa.InstanciarTilesAtaque(tilesAtaque);
                    seleccionandoTile = true;
                }
            }
        }
    }
}