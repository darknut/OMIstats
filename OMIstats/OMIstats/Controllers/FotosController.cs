using OMIstats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OMIstats.Controllers
{
    public class FotosController : BaseController
    {
        //
        // GET: /Fotos/

        public ActionResult Index(string clave, TipoOlimpiada tipo = TipoOlimpiada.OMI)
        {
            if (tipo == TipoOlimpiada.OMIP || tipo == TipoOlimpiada.OMIS)
                tipo = TipoOlimpiada.OMI;

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(clave, tipo);
            if (o == null)
                return RedirectTo(Pagina.ERROR, 404);

            List<Album> albumes = Models.Album.obtenerAlbumsDeOlimpiada(clave, tipo);

            if (!esAdmin())
            {
                if (albumes == null || albumes.Count == 0)
                    return RedirectTo(Pagina.ERROR, 404);

                if (albumes.Count == 1)
                    return RedirectTo(Pagina.ALBUM, albumes[0].id);
            }

            ViewBag.olimpiada = o;
            ViewBag.admin = esAdmin();
            ViewBag.olimpiadas = Olimpiada.obtenerOlimpiadas(tipo);
            ViewBag.olimpiadasConFotos = Models.Album.obtenerOlimpiadasConAlbumes(tipo);
            ViewBag.server = Server;

            return View(albumes);
        }

        //
        // GET: /Fotos/Album/

        public ActionResult Album(string id)
        {
            Models.Album album = Models.Album.obtenerAlbum(id);

            if (album == null || string.IsNullOrEmpty(album.id))
                return RedirectTo(Pagina.ERROR, 404);

            ViewBag.fotos = Foto.obtenerFotosDeAlbum(id);
            ViewBag.albumes = Models.Album.obtenerAlbumsDeOlimpiada(album.olimpiada, album.tipoOlimpiada);
            ViewBag.olimpiadas = Olimpiada.obtenerOlimpiadas(album.tipoOlimpiada);
            ViewBag.olimpiadasConFotos = Models.Album.obtenerOlimpiadasConAlbumes(album.tipoOlimpiada);

            return View(album);
        }

        //
        // GET: /Fotos/Edit/

        public ActionResult Edit(string omi, TipoOlimpiada tipo = TipoOlimpiada.OMI, string id = null)
        {
            if (!esAdmin())
                return RedirectTo(Pagina.ERROR, 504);

            Album al = Models.Album.obtenerAlbum(id);

            if (omi == null && !String.IsNullOrEmpty(id))
            {
                omi = al.olimpiada;
                tipo = al.tipoOlimpiada;
            }

            if (String.IsNullOrEmpty(id))
                al.update = true;

            if (tipo == TipoOlimpiada.OMIP || tipo == TipoOlimpiada.OMIS)
                tipo = TipoOlimpiada.OMI;

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(omi, tipo);
            if (o == null)
                return RedirectTo(Pagina.ERROR, 404);

            al.olimpiada = omi;
            al.tipoOlimpiada = tipo;

            return View(al);
        }

        //
        // POST: /Fotos/Edit/

        [HttpPost]
        public ActionResult Edit(Album album)
        {
            if (!esAdmin() || album == null)
                return RedirectTo(Pagina.HOME);

            Olimpiada o = Olimpiada.obtenerOlimpiadaConClave(album.olimpiada, album.tipoOlimpiada);
            if (o == null)
                return RedirectTo(Pagina.ERROR, 404);

            if (!ModelState.IsValid)
                return View(album);

            album.guardarDatos();

            Log.add(Log.TipoLog.FACEBOOK, "Álbum " + album.id + " actualizado por admin " + getUsuario().nombre);

            return RedirectTo(Pagina.FOTOS, album.olimpiada);
        }
    }
}
