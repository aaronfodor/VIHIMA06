CC = gcc
SRC = caff_parser.c
OUT = caff_parser.dll
OBJ = caff_parser.o 

caff_parser.dll: caff_parser.o
	$(CC) -shared -o $(OUT) $(OBJ) 

caff_parser.o: $(SRC) 
	$(CC) $(INC) -DBDLL -c $(SRC) -o $(OBJ)