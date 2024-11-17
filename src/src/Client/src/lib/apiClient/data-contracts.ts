/* eslint-disable */
/* tslint:disable */
/*
 * ---------------------------------------------------------------
 * ## THIS FILE WAS GENERATED VIA SWAGGER-TYPESCRIPT-API        ##
 * ##                                                           ##
 * ## AUTHOR: acacode                                           ##
 * ## SOURCE: https://github.com/acacode/swagger-typescript-api ##
 * ---------------------------------------------------------------
 */

export type IDomainEvent = object;

export interface LoginCommand {
  email?: string;
  password?: string;
}

export interface RegisterCommand {
  email?: string;
  full_name?: string;
  password?: string;
}

export enum Role {
  User = 'User',
  Staff = 'Staff',
  Admin = 'Admin',
}

export interface User {
  id?: string;
  /** @format date-time */
  created?: string | null;
  created_by?: string | null;
  /** @format date-time */
  last_modified?: string | null;
  last_modified_by?: string | null;
  /** @format date-time */
  deleted?: string | null;
  deleted_by?: string | null;
  domain_events?: IDomainEvent[];
  full_name: string;
  email: string;
  role: Role;
  password_hash: string;
}
