using System.Runtime.CompilerServices;
using AuctionService.Data;
using AuctionService.Data.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [Route("api/auctions")]
    [ApiController]
    public class AuctionsController : ControllerBase
    {
        private readonly AuctionDbContext _context;
        private readonly IMapper _mapper;

        public AuctionsController(AuctionDbContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
        {
            var auctions = await _context.Auctions
                .Include(x=>x.Item)
                .OrderBy(x => x.Item.Make)
                .ToListAsync();

                return _mapper.Map<List<AuctionDto>>(auctions);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuctionsById(Guid Id){
            var auction = await _context.Auctions
                    .Include(x=>x.Item)
                    .FirstOrDefaultAsync(y=> y.Id ==Id);

            if(auction == null) return NotFound();
            return _mapper.Map<AuctionDto>(auction);
        }
        [HttpPost]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
        {
            var auction =_mapper.Map<Auction>(auctionDto);
            //TODO: add current user as seller
            auction.Seller ="Test";

            _context.Auctions.Add(auction);

            var result = await _context.SaveChangesAsync() >0;
            if(!result) return BadRequest("Could not save chnages to the DB");

            return CreatedAtAction(nameof(GetAuctionsById),new{auction.Id},_mapper.Map<AuctionDto>(auction));
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAuction(Guid id,UpdateAuctionDto updateAuctionDto)
        {
            var auction =await _context.Auctions.Include(x=>x.Item)
                    .FirstOrDefaultAsync(y=>y.Id ==id);

            if(auction ==null) return NotFound();

            //TODO: check seller ===userName

            auction.Item.Make =updateAuctionDto.Make ?? auction.Item.Make;
            auction.Item.Model =updateAuctionDto.Model ?? auction.Item.Model;
            auction.Item.Color =updateAuctionDto.Color ?? auction.Item.Color;
            auction.Item.Mileage =updateAuctionDto.Mileage ?? auction.Item.Mileage;
            auction.Item.Year =updateAuctionDto.Year ?? auction.Item.Year;

            var result=await _context.SaveChangesAsync()>0;

            if(result) return Ok();

            return BadRequest("Problem saving changes");
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var auction =await _context.Auctions.FindAsync(id);

            if(auction == null) return NotFound();

            _context.Auctions.Remove(auction);
            var result = await _context.SaveChangesAsync()>0;
            if(!result) return BadRequest("Could not update DB");

            return Ok();
        }
    }
}
