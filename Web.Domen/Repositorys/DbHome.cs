﻿using System;
using System.Collections.Generic;
using System.Linq;
using Web.Domen.Abstract;
using Web.Domen.Infrastructure;
using Web.Domen.Models;

namespace Web.Domen.Repositorys
{
    public class DbHome : IHome
    {
        private readonly Context _context= new Context();

        public IEnumerable<PatyCategory> GetCategorys => _context.PatyCategories.ToList();

        public IEnumerable<Paty> GetPatys => _context.Paties.ToList();

        public Paty GetPaty(int id)
        {
            return _context.Paties.Find(id);
        }
    }
}