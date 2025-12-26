alter table types add column (color varchar(7) );

UPDATE types set color='#A8A878' where type_id=1 ;
UPDATE types set color='#F08030' where type_id=10 ;
UPDATE types set color='#6890F0' where type_id=11 ;
UPDATE types set color='#F8D030' where type_id=13 ;
UPDATE types set color='#78C850' where type_id=12 ;
UPDATE types set color='#98D8D8' where type_id=15 ;
UPDATE types set color='#C03028' where type_id=2 ;
UPDATE types set color='#A040A0' where type_id=4 ;
UPDATE types set color='#E0C068' where type_id=5 ;
UPDATE types set color='#A890F0' where type_id=3 ;
UPDATE types set color='#F85888' where type_id=14 ;
UPDATE types set color='#A8B820' where type_id=7 ;
UPDATE types set color='#B8A038' where type_id=6 ;
UPDATE types set color='#705898' where type_id=8 ;
UPDATE types set color='#7038F8' where type_id=16 ;
UPDATE types set color='#705848' where type_id=17 ;
UPDATE types set color='#B8B8D0' where type_id=9 ;
UPDATE types set color='#EE99AC' where type_id=18 ;


commit;