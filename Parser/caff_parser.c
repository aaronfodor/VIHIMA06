#include <stdio.h>
#include <stdlib.h>

#define BUFFER_SIZE 256 //minimum 36
#define PARSING_ERROR 3

char *filename;

int main(int argc, char *argv[])
{
    if( argc > 2 ) {
      printf("Tul sok argumentum.");
      exit(1);
    }
    else if( argc < 2){
      printf("Tul keves argumentum.");
      exit(1);
    }
    filename = argv[1];
    int ret = processFile();
    if(ret != 0) exit(ret);
}

int processFile()
{
    FILE *fp;
    unsigned char buffer[BUFFER_SIZE];

    fp = fopen(filename, "rb");
    if(fp == NULL)
    {
        printf("Hiba a CAFF fajl megnyitasa kozben.");
        return 2;
    }

    //generating the new file name in the given location
    int filename_length = 0;
    while(filename[filename_length] != '\0') filename_length++;
    char output_filename[filename_length-1];
    for(int j = 0; j < filename_length-4; j++) output_filename[j] = filename[j];
    output_filename[filename_length-4] = 'b';
    output_filename[filename_length-3] = 'm';
    output_filename[filename_length-2] = 'p';
    output_filename[filename_length-1] = '\0';
    filename = output_filename;

    //HEADER
    fread(buffer, 9, 1, fp); //id (1 byte) + length (8 byte)
    /*
        buffer[0] -> header 0x01
        buffer[1-8] -> header hossza
    */
    if(buffer[0] != 1) {printf("Hibas CAFF fajl."); return PARSING_ERROR;}
    int length = buffer[1] | (int)buffer[2] << 8
        | (int)buffer[3] << 16 | (int)buffer[4] << 24
        | (int)buffer[5] << 32 | (int)buffer[6] << 40
        | (int)buffer[7] << 48 | (int)buffer[8] << 56;

    if(length != 20) {printf("Hibas CAFF header."); return PARSING_ERROR;}
    fread(buffer, length, 1, fp); //header data olvasas
    /*
        buffer[0-3] -> 'CAFF' magic karakterek
        buffer[4-11] -> header size
        buffer[12-19] -> number of animated CIFFs
    */
    if(buffer[0] != 'C' || buffer[1] != 'A' || buffer[2] != 'F' || buffer[3] != 'F') {printf("Hibas CAFF header."); return PARSING_ERROR;}

    int header_size = buffer[4] | (int)buffer[5] << 8
        | (int)buffer[6] << 16 | (int)buffer[7] << 24
        | (int)buffer[8] << 32 | (int)buffer[9] << 40
        | (int)buffer[10] << 48 | (int)buffer[11] << 56;

    if(header_size != length) {printf("Hibas CAFF header."); return PARSING_ERROR;}

    int num_anim = buffer[12] | (int)buffer[13] << 8
        | (int)buffer[14] << 16 | (int)buffer[15] << 24
        | (int)buffer[16] << 32 | (int)buffer[17] << 40
        | (int)buffer[18] << 48 | (int)buffer[19] << 56;

    int creditsread = 0;
    int animread = 0;
    //credits es animation blokkok olvasása
    for(int i = 0; i < num_anim+1; i++)
    {
        fread(buffer, 9, 1, fp); //id+length
        /*
            buffer[0] -> block id
            buffer[1-8] -> block hossza
        */
        length = buffer[1] | (int)buffer[2] << 8
            | (int)buffer[3] << 16 | (int)buffer[4] << 24
            | (int)buffer[5] << 32 | (int)buffer[6] << 40
            | (int)buffer[7] << 48 | (int)buffer[8] << 56;

        //buffer[0] -> 0x02
        if(buffer[0] == 2)
        {
            if(creditsread == 1) {printf("Hibas CAFF fajl."); remove(filename); return PARSING_ERROR;} //ha tobb credits blokk van mint 1, akkor hibat dob
            int ret = readCredits(fp, length);
            if(ret != 0) {printf("Hibas CAFF fajl."); remove(filename); return PARSING_ERROR;}
            creditsread = 1; //credits kiolvasasra kerult
        }
        //buffer[0] -> 0x03
        else if(buffer[0] == 3 && animread == 0) // azert, hogy csak az elso kepet olvassa ki
        {
            fseek(fp, 8, SEEK_CUR); //animation duration atugrasa
            int ret = readCIFF(fp, length-8); //CIFF fajl olvasasa, a CIFF merete a blokk hossza a duration nelkul
            if(ret != 0) {printf("Hibas CAFF fajl."); return PARSING_ERROR;}
            animread = 1; //kep kiolvasasra es kiirasra kerult
        }
        else {printf("Hibas CAFF fajl."); remove(filename); return PARSING_ERROR;}
        if(creditsread && animread) break;
    }
    fclose(fp);
    return 0;
}

int readCredits(FILE *fp, int length)
{
    unsigned char buffer[BUFFER_SIZE];
    fread(buffer, 14, 1, fp); //credits data olvasas
    /*
        buffer[0-1] -> year
        buffer[2] -> month
        buffer[3] -> day
        buffer[4] -> hour
        buffer[5] -> minute
        buffer[6-13] -> length of creator
        buffer[creator_len-14] -> creator
    */
    int year = buffer[0] | (int)buffer[1] << 8;
    if(year < 2000) return 1;
    if(buffer[2] < 1 || buffer[2] > 12) return 1;
    if(buffer[3] < 1 || buffer[3] > 31) return 1;
    if(buffer[4] < 0 || buffer[4] > 23) return 1;
    if(buffer[5] < 0 || buffer[5] > 59) return 1;

    int creator_len = buffer[6] | (int)buffer[7] << 8
        | (int)buffer[8] << 16 | (int)buffer[9] << 24
        | (int)buffer[10] << 32 | (int)buffer[11] << 40
        | (int)buffer[12] << 48 | (int)buffer[13] << 56;

    if(length-14 != creator_len) return 1;
    fseek(fp, creator_len, SEEK_CUR); //creator adat atugrasa
    return 0;
}

int readCIFF(FILE *fp, int file_size)
{
    unsigned char buffer[BUFFER_SIZE];

    fread(buffer, 36, 1, fp); //1db 4 byte-os es 4 db 8 byte-os tartalom
    /*
        buffer[0-3] -> magic 'CIFF' karakterek
        buffer[4-11] -> header size
        buffer[12-19] -> content size
        buffer[20-27] -> width
        buffer[28-35] -> height
    */
    if(buffer[0] != 'C' || buffer[1] != 'I' || buffer[2] != 'F' || buffer[3] != 'F') {printf("Hibas CIFF header."); return PARSING_ERROR;}
    int header_size = buffer[4] | (int)buffer[5] << 8
        | (int)buffer[6] << 16 | (int)buffer[7] << 24
        | (int)buffer[8] << 32 | (int)buffer[9] << 40
        | (int)buffer[10] << 48 | (int)buffer[11] << 56;

    int content_size = buffer[12] | (int)buffer[13] << 8
        | (int)buffer[14] << 16 | (int)buffer[15] << 24
        | (int)buffer[16] << 32 | (int)buffer[17] << 40
        | (int)buffer[18] << 48 | (int)buffer[19] << 56;

    if(file_size-header_size != content_size) {printf("Hibas CIFF fajl."); return PARSING_ERROR;}

    int width = buffer[20] | (int)buffer[21] << 8
        | (int)buffer[22] << 16 | (int)buffer[23] << 24
        | (int)buffer[24] << 32 | (int)buffer[25] << 40
        | (int)buffer[26] << 48 | (int)buffer[27] << 56;

    int height = buffer[28] | (int)buffer[29] << 8
        | (int)buffer[30] << 16 | (int)buffer[31] << 24
        | (int)buffer[32] << 32 | (int)buffer[33] << 40
        | (int)buffer[34] << 48 | (int)buffer[35] << 56;

    if(content_size != width*height*3) {printf("Hibas CIFF fajl."); return PARSING_ERROR;}

    fseek(fp, header_size-36, SEEK_CUR); //a headerbol mar ki lett olvasva a fix 36 byte a maradek atugrasa

    createBMP(fp, content_size, width, height);
    return 0;
}

void createBMP(FILE *fp, int image_size, int width, int height)
{
    FILE *fp2;
    fp2 = fopen(filename, "wb");
    unsigned char buffer[BUFFER_SIZE];

    int offset = 26; //file header es bitmap header
    int file_size = image_size + offset;

    //File header
    unsigned char file_header[14];
    file_header[0] = 'B'; //mindig B
    file_header[1] = 'M'; //mindig M
    file_header[2] = file_size & 0xFF; //a teljes file merete (4 byte)
    file_header[3] = (file_size >> 8) & 0xFF;
    file_header[4] = (file_size >> 16) & 0xFF;
    file_header[5] = (file_size >> 24) & 0xFF;
    file_header[6] = 0; //mindig 0
    file_header[7] = 0; //mindig 0
    file_header[8] = 0; //mindig 0
    file_header[9] = 0; //mindig 0
    file_header[10] = offset; //file offset, hogy hol kezdodik az adat (4 byte)
    file_header[11] = 0; //file offset
    file_header[12] = 0; //file offset
    file_header[13] = 0; //file offset

    fwrite(file_header, 14, 1, fp2);

    //Bitmap header
    unsigned char bitmap_header[12];
    bitmap_header[0] = 12; //header hossza (4 byte)
    bitmap_header[1] = 0; //header hossza
    bitmap_header[2] = 0; //header hossza
    bitmap_header[3] = 0; //header hossza
    bitmap_header[4] = width & 0xFF; //width (2 byte)
    bitmap_header[5] = (width >> 8) & 0xFF;
    bitmap_header[6] = height & 0xFF; //height (2 byte)
    bitmap_header[7] = (height >> 8) & 0xFF;
    bitmap_header[8] = 1; //mindig 1 (2 byte)
    bitmap_header[9] = 0; //elozo miatt 0
    bitmap_header[10] = 24; //bits per pixel (2 byte)
    bitmap_header[11] = 0; //elozo miatt 0

    fwrite(bitmap_header, 12, 1, fp2);

    //pixelek kiolvasasa visszafele, hogy RGB helyett BGR legyen es a kep is megforduljon, mert lentrol felfele rajzol a bmp
    fseek(fp, image_size-BUFFER_SIZE, SEEK_CUR);
    unsigned char reverse_buffer[BUFFER_SIZE];
    int cicles = image_size/BUFFER_SIZE; //hanyszor lehet BUFFER_SIZE-nyi byte-ot kiirni
    int remainder_bytes = image_size%BUFFER_SIZE; //ha BUFFER_SIZE nem egesz tobbszorose az image_size-nak
    //egesz bufferek kiirasa
    for(int i = 0; i < cicles; i++)
    {

        fread(buffer, BUFFER_SIZE, 1, fp);
        for(int j = 0; j < BUFFER_SIZE; j++) reverse_buffer[BUFFER_SIZE-1-j] = buffer[j]; //buffer megforditasa
        fwrite(reverse_buffer, BUFFER_SIZE, 1, fp2); //egy BUFFER_SIZE file-ba irasa
        fseek(fp, -2*BUFFER_SIZE, SEEK_CUR);
    }

    //maradek byte-ok kiirasa
    if(remainder_bytes > 0)
    {
        fread(buffer, BUFFER_SIZE, 1, fp);
        for(int j = 0; j < BUFFER_SIZE; j++) reverse_buffer[BUFFER_SIZE-1-j] = buffer[j]; //buffer megfordítása
        fwrite(reverse_buffer, remainder_bytes, 1, fp2);
    }

    fclose(fp2);
}

__declspec(dllexport) int parse(char* path)
{
    filename = path;
    int ret = processFile();
    if(ret != 0) return ret;
    else return 0;
}
