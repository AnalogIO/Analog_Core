﻿using System.Collections.Generic;
using System.Linq;
using CoffeeCard.Models;

namespace CoffeeCard.Services
{
    public class ProgrammeService : IProgrammeService
    {
        private readonly CoffeecardContext _context;

        public ProgrammeService(CoffeecardContext context)
        {
            _context = context;
        }

        public IEnumerable<Programme> GetProgrammes()
        {
            return _context.Programmes.AsEnumerable();
        }

        public Programme GetProgramme(int id)
        {
            return _context.Programmes.FirstOrDefault(x => x.Id == id);
        }
    }
}