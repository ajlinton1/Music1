using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MusicPlayerWeb.Models;

namespace MusicPlayerWeb.Controllers
{
    public class SegmentController : ApiController
    {
        public SegmentController()
        {
        }

        public IList<Segment> Get(int skip)
        {
            int maxReturn = 100;
            var segmentRepository = new SegmentRepository();
            return segmentRepository.Get(maxReturn, skip);
        }

    }
}
