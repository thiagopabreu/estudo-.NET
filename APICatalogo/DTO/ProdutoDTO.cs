﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTO
{
    public class ProdutoDTO
    {
        public int ProdutoId { get; set; }
        public string? Nome { get; set; }
        public string? ImagemUrl { get; set; }
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }
        public int CategoriaId { get; set; }
    }
}

