# -*- coding: utf-8 -*-
"""
Created on Sat Oct  7 20:27:56 2017

@author: soomin
"""

#!/usr/bin/python
import csv
from apiclient.discovery import build
from apiclient.errors import HttpError
from oauth2client.tools import argparser


# Set DEVELOPER_KEY to the API key value from the APIs & auth > Registered apps
# tab of
#   https://cloud.google.com/consolyoutube_liste
# Please ensure that you have enabled the YouTube Data API for your project.
DEVELOPER_KEY = "AIzaSyCd6zURiUu7ohktj1uAn4UJMR_jA_7bbds"
YOUTUBE_API_SERVICE_NAME = "youtube"
YOUTUBE_API_VERSION = "v3"

def youtube_search(options):
  youtube = build(YOUTUBE_API_SERVICE_NAME, YOUTUBE_API_VERSION,
    developerKey=DEVELOPER_KEY)

  # Call the search.list method to retrieve results matching the specified
  # query term.
  search_response = youtube.search().list(
    q=options.q,
    part="id,snippet",
    maxResults=options.max_results
  ).execute()

  videos = []
  

  # Add each result to the appropriate list, and then display the lists of
  # matching videos, channels, and playlists.
  for search_result in search_response.get("items", []):
    if search_result["id"]["kind"] == "youtube#video":
      videos.append([search_result["snippet"]["title"],
                                 search_result["id"]["videoId"], 
                                 search_result["snippet"]["thumbnails"]["medium"]["url"],                                 
                                 search_result["snippet"]["channelTitle"]                         
                                 
                                 ])
  i=0;
  for data in videos:
      videos_list = youtube.videos().list(
          part = "contentDetails, statistics",
          id = data[1]
          ).execute().get("items",[])      
      videos[i].append(videos_list[0]["contentDetails"]["duration"])
      videos[i].append(videos_list[0]["statistics"]["viewCount"])
      
      i = i+1
      
  '''    
  for data in videos:
      print(data)
      print('\n')
  '''
  return videos  
 
        
        #title, id, url, channel, duration, viewcount 
        
  #print ("Videos:\n", "\n".join(videos), "\n")
  #print ("Channels:\n", "\n".join(channels), "\n")
  #print ("Playlists:\n", "\n".join(playlists), "\n")

def main(search_word):
    print(search_word)
    argparser.add_argument("--q", help="Search term", default=search_word)
    argparser.add_argument("--max-results", help="Max results", default=25)
    args = argparser.parse_args()

    try:
        youtube_list = youtube_search(args)
    except HttpError as e:
        print ( "An HTTP error %d occurred:\n%s" % (e.resp.status, e.content))
        
    return youtube_list