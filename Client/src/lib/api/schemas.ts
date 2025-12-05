import { makeApi, Zodios, type ZodiosOptions } from "@zodios/core";
import { z } from "zod";

const MockLoginRequest = z
  .object({ username: z.string().nullable(), password: z.string().nullable() })
  .partial();
const PokemonType = z.union([
  z.literal(0),
  z.literal(1),
  z.literal(2),
  z.literal(3),
  z.literal(4),
  z.literal(5),
  z.literal(6),
  z.literal(7),
  z.literal(8),
  z.literal(9),
  z.literal(10),
  z.literal(11),
  z.literal(12),
  z.literal(13),
  z.literal(14),
  z.literal(15),
  z.literal(16),
  z.literal(17),
]);
const Category = z.union([
  z.literal(0),
  z.literal(1),
  z.literal(2),
  z.literal(3),
  z.literal(4),
  z.literal(5),
  z.literal(6),
  z.literal(7),
  z.literal(8),
  z.literal(9),
]);
const DamageClass = z.union([z.literal(0), z.literal(1), z.literal(2)]);
const Ailment = z.union([
  z.literal(0),
  z.literal(1),
  z.literal(2),
  z.literal(3),
  z.literal(4),
  z.literal(5),
  z.literal(6),
]);
const PokemonStat = z.union([
  z.literal(0),
  z.literal(1),
  z.literal(2),
  z.literal(3),
  z.literal(4),
  z.literal(5),
  z.literal(6),
]);
const StatChange = z
  .object({ stat: PokemonStat, change: z.number().int() })
  .partial();
const Move = z
  .object({
    moveId: z.number().int(),
    name: z.string().nullable(),
    type: PokemonType,
    category: Category,
    damageClass: DamageClass,
    power: z.number().int(),
    accuracy: z.number().int(),
    pp: z.number().int(),
    priority: z.number().int(),
    target: z.string().nullable(),
    statChance: z.number().int(),
    ailment: Ailment,
    ailmentChance: z.number().int(),
    healing: z.number().int(),
    drain: z.number().int(),
    critRate: z.number().int(),
    statChanges: z.array(StatChange).nullable(),
  })
  .partial();
const PokemonSpecies = z
  .object({
    pokemonSpeciesId: z.number().int(),
    name: z.string().nullable(),
    frontImage: z.string().nullable(),
    backImage: z.string().nullable(),
    type1: PokemonType,
    type2: PokemonType,
    evolveLevel: z.number().int(),
    baseHp: z.number().int(),
    baseAttack: z.number().int(),
    baseDefence: z.number().int(),
    baseSpecialAttack: z.number().int(),
    baseSpecialDefence: z.number().int(),
    baseSpeed: z.number().int(),
    moveList: z.array(Move).nullable(),
  })
  .partial();
const Pokemon = z
  .object({
    pokemonId: z.string().nullable(),
    pokemonSpeciesId: z.number().int(),
    species: PokemonSpecies,
    level: z.number().int(),
    exp: z.number().int(),
    moves: z.array(Move).nullable(),
  })
  .partial();
const PlayerDto = z
  .object({ name: z.string().nullable(), iconUrl: z.string().nullable() })
  .partial();
const SelectStarterRequest = z
  .object({ pokemonSpeciesId: z.number().int() })
  .partial();

export const schemas = {
  MockLoginRequest,
  PokemonType,
  Category,
  DamageClass,
  Ailment,
  PokemonStat,
  StatChange,
  Move,
  PokemonSpecies,
  Pokemon,
  PlayerDto,
  SelectStarterRequest,
};

const endpoints = makeApi([
  {
    method: "post",
    path: "/api/Auth/login/mock",
    alias: "postApiAuthloginmock",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: MockLoginRequest,
      },
    ],
    response: z.void(),
  },
  {
    method: "post",
    path: "/api/Auth/logout",
    alias: "postApiAuthlogout",
    requestFormat: "json",
    response: z.void(),
  },
  {
    method: "get",
    path: "/api/Auth/status",
    alias: "getApiAuthstatus",
    requestFormat: "json",
    response: z.void(),
  },
  {
    method: "get",
    path: "/api/Battle/:battleId",
    alias: "getApiBattleBattleId",
    requestFormat: "json",
    parameters: [
      {
        name: "battleId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: z.void(),
  },
  {
    method: "post",
    path: "/api/Battle/cpu",
    alias: "postApiBattlecpu",
    requestFormat: "json",
    response: z.void(),
  },
  {
    method: "post",
    path: "/api/Gacha/pull",
    alias: "postApiGachapull",
    requestFormat: "json",
    response: Pokemon,
  },
  {
    method: "get",
    path: "/api/Moves",
    alias: "getApiMoves",
    requestFormat: "json",
    response: z.void(),
  },
  {
    method: "get",
    path: "/api/Moves/:id",
    alias: "getApiMovesId",
    requestFormat: "json",
    parameters: [
      {
        name: "id",
        type: "Path",
        schema: z.number().int(),
      },
    ],
    response: z.void(),
  },
  {
    method: "get",
    path: "/api/Party",
    alias: "getApiParty",
    requestFormat: "json",
    response: z.array(Pokemon),
  },
  {
    method: "delete",
    path: "/api/Party/:pokemonId",
    alias: "deleteApiPartyPokemonId",
    requestFormat: "json",
    parameters: [
      {
        name: "pokemonId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: z.void(),
  },
  {
    method: "get",
    path: "/api/Player/me",
    alias: "getApiPlayerme",
    requestFormat: "json",
    response: z.void(),
  },
  {
    method: "post",
    path: "/api/Player/me",
    alias: "postApiPlayerme",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: PlayerDto,
      },
    ],
    response: z.void(),
  },
  {
    method: "get",
    path: "/api/Pokemon",
    alias: "getApiPokemon",
    requestFormat: "json",
    response: z.void(),
  },
  {
    method: "get",
    path: "/api/Pokemon/:id",
    alias: "getApiPokemonId",
    requestFormat: "json",
    parameters: [
      {
        name: "id",
        type: "Path",
        schema: z.number().int(),
      },
    ],
    response: z.void(),
  },
  {
    method: "get",
    path: "/api/Starter/options",
    alias: "getApiStarteroptions",
    requestFormat: "json",
    response: z.array(PokemonSpecies),
  },
  {
    method: "post",
    path: "/api/Starter/select",
    alias: "postApiStarterselect",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: z.object({ pokemonSpeciesId: z.number().int() }).partial(),
      },
    ],
    response: z.void(),
  },
]);

export const api = new Zodios(endpoints);

export function createApiClient(baseUrl: string, options?: ZodiosOptions) {
  return new Zodios(baseUrl, endpoints, options);
}
