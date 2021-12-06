/*
Copyright 2005 - 2021 Advantage Solutions, s. r. o.

This file is part of ORIGAM (http://www.origam.org).

ORIGAM is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

ORIGAM is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with ORIGAM. If not, see <http://www.gnu.org/licenses/>.
*/
import { createWriteStream } from "fs";
import { https } from 'follow-redirects';
import decompress = require('decompress');

export function download(options: any,fullPathFileName:string)
{
	https.get(options)
		.on('response', (response) => {
			if(response.statusCode===200)
			{
   			 response.pipe(createWriteStream(fullPathFileName));
			}
			else
			{
				console.error("Problem with file. " + response.statusMessage);
			}
			});
}

export async function downloadsync (options: any,fullPathFileName:string) 
{
	try {
        await downloadPage(options,fullPathFileName);
    } catch (error) {
        console.error('ERROR:');
        console.error(error);
    }
} 

export async function unzipfile(filename: string,path:string){
		await decompress(filename, path);
	console.log('Unzip done!');
  }
function downloadPage(options: any,fullPathFileName:string) {
	return new Promise((resolve, reject) => {
		https.get(options)
			.on('response', (response) => {
				if(response.statusCode===200)
				{
				response.pipe(createWriteStream(fullPathFileName))
				.on('finish',()=>{resolve("done");});
				}
				else
				{
					console.error("Problem with file. " + response.statusMessage);
				}
				})
			.on('error',(error) =>{reject(error);})
			;
			});
}

